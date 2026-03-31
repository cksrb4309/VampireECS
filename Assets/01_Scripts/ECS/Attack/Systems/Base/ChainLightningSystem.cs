using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(AuraSystem))]
[UpdateAfter(typeof(ProjectileSpawnSystem))]
public partial struct ChainLightningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ChainLightningData>();
        state.RequireForUpdate<ChainLightningStatsData>();
        state.RequireForUpdate<SpatialIndex>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;

        SpatialIndex spatialIndex = SystemAPI.GetSingleton<SpatialIndex>();
        ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        ComponentLookup<FactionData> factionLookup = SystemAPI.GetComponentLookup<FactionData>(true);
        ComponentLookup<HealthData> healthLookup = SystemAPI.GetComponentLookup<HealthData>(true);
        ComponentLookup<DeadTag> deadLookup = SystemAPI.GetComponentLookup<DeadTag>(true);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        NativeList<Entity> candidates = new NativeList<Entity>(Allocator.Temp);
        NativeList<Entity> hitTargets = new NativeList<Entity>(Allocator.Temp);

        foreach (var (transformRO, chainRW, combatStatsRO, chainStatsRO, sourceEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<ChainLightningData>,
                RefRO<CombatStatsData>,
                RefRO<ChainLightningStatsData>>()
            .WithEntityAccess())
        {
            ref ChainLightningData chainData = ref chainRW.ValueRW;
            ref readonly CombatStatsData combatStats = ref combatStatsRO.ValueRO;
            ref readonly ChainLightningStatsData chainStats = ref chainStatsRO.ValueRO;

            chainData.ElapsedTime += deltaTime * chainStats.AttackSpeed * combatStats.AttackSpeed;

            if (chainData.ElapsedTime < 1f)
                continue;

            int castCount = (int)chainData.ElapsedTime;
            chainData.ElapsedTime %= 1f;

            float3 sourcePosition = transformRO.ValueRO.Position;

            for (int castIndex = 0; castIndex < castCount; castIndex++)
            {
                ExecuteChain(
                    sourceEntity,
                    sourcePosition,
                    chainData.OwnerFaction,
                    chainStats,
                    combatStats,
                    spatialIndex,
                    transformLookup,
                    factionLookup,
                    healthLookup,
                    deadLookup,
                    ref candidates,
                    ref hitTargets,
                    ref ecb);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        candidates.Dispose();
        hitTargets.Dispose();
    }

    private static void ExecuteChain(
        Entity sourceEntity,
        float3 sourcePosition,
        Faction ownerFaction,
        in ChainLightningStatsData chainStats,
        in CombatStatsData combatStats,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        ref NativeList<Entity> candidates,
        ref NativeList<Entity> hitTargets,
        ref EntityCommandBuffer ecb)
    {
        float acquireRadius = chainStats.AcquireRadius * combatStats.AttackRange;
        float jumpRadius = chainStats.JumpRadius * combatStats.AttackRange;
        int maxTargets = math.max(1, chainStats.MaxTargets);
        candidates.Clear();
        hitTargets.Clear();

        Entity currentSource = sourceEntity;
        float3 currentPosition = sourcePosition;
        bool firstHop = true;

        for (int hopIndex = 0; hopIndex < maxTargets; hopIndex++)
        {
            float searchRadius = firstHop ? acquireRadius : jumpRadius;

            Entity nextTarget = FindNearestTarget(
                currentSource,
                currentPosition,
                searchRadius,
                ownerFaction,
                hitTargets,
                spatialIndex,
                transformLookup,
                factionLookup,
                healthLookup,
                deadLookup,
                ref candidates);

            if (nextTarget == Entity.Null)
                break;

            float damageMultiplier = math.pow(chainStats.DamageMultiplierPerJump, hopIndex);
            float finalDamage = chainStats.Damage * combatStats.Damage * damageMultiplier;

            Entity damageEventEntity = ecb.CreateEntity();
            ecb.AddComponent(damageEventEntity, new DamageEventData
            {
                Target = nextTarget,
                Damage = finalDamage
            });

            hitTargets.Add(nextTarget);
            currentSource = nextTarget;
            currentPosition = transformLookup[nextTarget].Position;
            firstHop = false;
        }
    }

    private static Entity FindNearestTarget(
        Entity excludedEntity,
        float3 center,
        float radius,
        Faction ownerFaction,
        in NativeList<Entity> alreadyHitTargets,
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

            if (ContainsEntity(alreadyHitTargets, candidate))
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

    private static bool ContainsEntity(in NativeList<Entity> entities, Entity candidate)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i] == candidate)
                return true;
        }

        return false;
    }
}
