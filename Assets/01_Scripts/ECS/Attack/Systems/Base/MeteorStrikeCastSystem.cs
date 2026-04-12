using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(ChainLightningSystem))]
[UpdateAfter(typeof(SpatialPartitionBuildSystem))]
public partial struct MeteorStrikeCastSystem : ISystem
{
    private const float BurstWindowFraction = 0.5f;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MeteorStrikeData>();
        state.RequireForUpdate<MeteorStrikeBaseStatsData>();
        state.RequireForUpdate<MeteorStrikeStatsData>();
        state.RequireForUpdate<SpatialIndex>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;

        state.Dependency.Complete();

        SpatialIndex spatialIndex = SystemAPI.GetSingleton<SpatialIndex>();
        ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        ComponentLookup<FactionData> factionLookup = SystemAPI.GetComponentLookup<FactionData>(true);
        ComponentLookup<HealthData> healthLookup = SystemAPI.GetComponentLookup<HealthData>(true);
        ComponentLookup<DeadTag> deadLookup = SystemAPI.GetComponentLookup<DeadTag>(true);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        NativeList<Entity> candidates = new NativeList<Entity>(Allocator.Temp);

        foreach (var (transformRO, meteorRW, combatStatsRO, meteorBaseStatsRO, meteorStatsRO, sourceEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<MeteorStrikeData>,
                RefRO<CombatStatsData>,
                RefRO<MeteorStrikeBaseStatsData>,
                RefRO<MeteorStrikeStatsData>>()
            .WithEntityAccess())
        {
            ref MeteorStrikeData meteorData = ref meteorRW.ValueRW;
            ref readonly CombatStatsData combatStats = ref combatStatsRO.ValueRO;
            ref readonly MeteorStrikeBaseStatsData meteorBaseStats = ref meteorBaseStatsRO.ValueRO;
            ref readonly MeteorStrikeStatsData meteorStats = ref meteorStatsRO.ValueRO;

            float finalAttackSpeed =
                meteorBaseStats.BaseAttackSpeed *
                (1f + meteorStats.AttackSpeedBonusRate) *
                combatStats.AttackSpeed;

            meteorData.ElapsedTime += deltaTime * finalAttackSpeed;

            if (meteorData.ElapsedTime < 1f)
                continue;

            int castCount = (int)meteorData.ElapsedTime;
            meteorData.ElapsedTime %= 1f;

            float3 sourcePosition = transformRO.ValueRO.Position;

            for (int castIndex = 0; castIndex < castCount; castIndex++)
            {
                ExecuteCast(
                    sourceEntity,
                    sourcePosition,
                    meteorData.OwnerFaction,
                    meteorBaseStats,
                    meteorStats,
                    combatStats,
                    spatialIndex,
                    transformLookup,
                    factionLookup,
                    healthLookup,
                    deadLookup,
                    castIndex,
                    ref candidates,
                    ref ecb);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        candidates.Dispose();
    }

    private static void ExecuteCast(
        Entity sourceEntity,
        float3 sourcePosition,
        Faction ownerFaction,
        in MeteorStrikeBaseStatsData meteorBaseStats,
        in MeteorStrikeStatsData meteorStats,
        in CombatStatsData combatStats,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        int castIndex,
        ref NativeList<Entity> candidates,
        ref EntityCommandBuffer ecb)
    {
        float acquireRadius = (meteorBaseStats.BaseAcquireRadius + meteorStats.AcquireRadiusBonus) * combatStats.AttackRange;
        float effectiveAttackSpeed =
            math.max(
                0.0001f,
                meteorBaseStats.BaseAttackSpeed *
                (1f + meteorStats.AttackSpeedBonusRate) *
                combatStats.AttackSpeed);

        float attackInterval = 1f / effectiveAttackSpeed;

        Entity target = FindNearestTarget(
            sourceEntity,
            sourcePosition,
            acquireRadius,
            ownerFaction,
            spatialIndex,
            transformLookup,
            factionLookup,
            healthLookup,
            deadLookup,
            ref candidates);

        if (target == Entity.Null)
            return;

        float3 targetPosition = transformLookup[target].Position;
        int meteorCount = math.max(1, meteorBaseStats.BaseMeteorCount + meteorStats.MeteorCountBonus);
        float impactRadius = (meteorBaseStats.BaseImpactRadius + meteorStats.ImpactRadiusBonus) * combatStats.AttackRange;
        float scatterRadius = (meteorBaseStats.BaseScatterRadius + meteorStats.ScatterRadiusBonus) * combatStats.AttackRange;
        float baseDelay = math.max(0.05f, meteorBaseStats.BaseImpactDelay + meteorStats.ImpactDelayBonus);
        float burstWindow = attackInterval * BurstWindowFraction;
        float burstStepDelay = CalculateBurstStepDelay(meteorCount, burstWindow);
        float finalDamage =
            meteorBaseStats.BaseDamage *
            (1f + meteorStats.DamageBonusRate) *
            combatStats.Damage;

        for (int meteorIndex = 0; meteorIndex < meteorCount; meteorIndex++)
        {
            float3 impactPosition = targetPosition;
            float impactDelayOffset = burstStepDelay * meteorIndex;
            float totalImpactDelay = baseDelay + impactDelayOffset;

            if (meteorCount > 1 && scatterRadius > 0f)
            {
                float angle = ((castIndex * 0.37f) + meteorIndex) * math.PI * 2f / meteorCount;
                float distance = scatterRadius * (meteorCount == 1 ? 0f : (meteorIndex + 1f) / meteorCount);
                impactPosition += new float3(math.cos(angle), 0f, math.sin(angle)) * distance;
            }

            Entity pendingMeteorEntity = ecb.CreateEntity();
            ecb.AddComponent(pendingMeteorEntity, new MeteorStrikePendingData
            {
                Position = impactPosition,
                Damage = finalDamage,
                Radius = impactRadius,
                RemainingDelay = totalImpactDelay,
                OwnerFaction = ownerFaction
            });

            Entity telegraphEventEntity = ecb.CreateEntity();
            ecb.AddComponent(telegraphEventEntity, new MeteorStrikeTelegraphVisualEvent
            {
                Position = impactPosition,
                Radius = impactRadius,
                Duration = totalImpactDelay
            });
        }
    }

    private static float CalculateBurstStepDelay(int meteorCount, float burstWindow)
    {
        if (meteorCount <= 1 || burstWindow <= 0f)
        {
            return 0f;
        }

        return burstWindow / (meteorCount - 1);
    }

    private static Entity FindNearestTarget(
        Entity excludedEntity,
        float3 center,
        float radius,
        Faction ownerFaction,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        ref NativeList<Entity> candidates)
    {
        SpatialUtility.QueryRadius(center, radius, spatialIndex.CellSize, spatialIndex.Map, ref candidates);

        float radiusSq = radius * radius;
        float nearestDistanceSq = float.MaxValue;
        Entity nearestTarget = Entity.Null;

        for (int i = 0; i < candidates.Length; i++)
        {
            Entity candidate = candidates[i];

            if (candidate == excludedEntity)
                continue;

            if (!transformLookup.HasComponent(candidate) ||
                !factionLookup.HasComponent(candidate) ||
                !healthLookup.HasComponent(candidate))
                continue;

            if (deadLookup.HasComponent(candidate))
                continue;

            if (factionLookup[candidate].Value == ownerFaction)
                continue;

            float3 delta = transformLookup[candidate].Position - center;
            float distanceSq = math.lengthsq(delta);

            if (distanceSq > radiusSq)
                continue;

            if (distanceSq >= nearestDistanceSq)
                continue;

            nearestDistanceSq = distanceSq;
            nearestTarget = candidate;
        }

        return nearestTarget;
    }
}
