using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(MeteorStrikeCastSystem))]
[UpdateAfter(typeof(SpatialPartitionBuildSystem))]
public partial struct BlackHoleCastSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BlackHoleData>();
        state.RequireForUpdate<BlackHoleStatsData>();
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

        foreach (var (transformRO, blackHoleRW, combatStatsRO, blackHoleStatsRO, sourceEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<BlackHoleData>,
                RefRO<CombatStatsData>,
                RefRO<BlackHoleStatsData>>()
            .WithEntityAccess())
        {
            ref BlackHoleData blackHoleData = ref blackHoleRW.ValueRW;
            ref readonly CombatStatsData combatStats = ref combatStatsRO.ValueRO;
            ref readonly BlackHoleStatsData blackHoleStats = ref blackHoleStatsRO.ValueRO;

            blackHoleData.ElapsedTime += deltaTime * blackHoleStats.AttackSpeed * combatStats.AttackSpeed;

            if (blackHoleData.ElapsedTime < 1f)
                continue;

            int castCount = (int)blackHoleData.ElapsedTime;
            blackHoleData.ElapsedTime %= 1f;

            float3 sourcePosition = transformRO.ValueRO.Position;

            for (int castIndex = 0; castIndex < castCount; castIndex++)
            {
                ExecuteCast(
                    sourceEntity,
                    sourcePosition,
                    blackHoleData.OwnerFaction,
                    blackHoleStats,
                    combatStats,
                    spatialIndex,
                    transformLookup,
                    factionLookup,
                    healthLookup,
                    deadLookup,
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
        in BlackHoleStatsData blackHoleStats,
        in CombatStatsData combatStats,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        ref NativeList<Entity> candidates,
        ref EntityCommandBuffer ecb)
    {
        float acquireRadius = blackHoleStats.AcquireRadius * combatStats.AttackRange;

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

        Entity blackHoleEntity = ecb.CreateEntity();
        float totalDuration = math.max(0.1f, blackHoleStats.Duration);
        ecb.AddComponent(blackHoleEntity, new BlackHoleFieldData
        {
            Position = targetPosition,
            Radius = blackHoleStats.Radius * combatStats.AttackRange,
            PullStrength = blackHoleStats.PullStrength,
            TickDamage = blackHoleStats.Damage * combatStats.Damage,
            TickInterval = math.max(0.05f, blackHoleStats.TickInterval),
            TickElapsedTime = 0f,
            TotalDuration = totalDuration,
            RemainingDuration = totalDuration,
            OwnerFaction = ownerFaction
        });
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
