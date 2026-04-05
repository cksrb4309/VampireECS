using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(OrbitMoveSystem))]
[UpdateAfter(typeof(SpatialPartitionBuildSystem))]
public partial struct OrbitHitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OrbitProjectileData>();
        state.RequireForUpdate<SpatialIndex>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        SpatialIndex spatialIndex = SystemAPI.GetSingleton<SpatialIndex>();
        ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        ComponentLookup<FactionData> factionLookup = SystemAPI.GetComponentLookup<FactionData>(true);
        ComponentLookup<HealthData> healthLookup = SystemAPI.GetComponentLookup<HealthData>(true);
        ComponentLookup<DeadTag> deadLookup = SystemAPI.GetComponentLookup<DeadTag>(true);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        NativeList<Entity> candidates = new NativeList<Entity>(Allocator.Temp);

        foreach (var (transformRO, projectileRO, recentHits) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<OrbitProjectileData>,
                DynamicBuffer<OrbitRecentHitData>>())
        {
            ref readonly LocalTransform transform = ref transformRO.ValueRO;
            ref readonly OrbitProjectileData projectile = ref projectileRO.ValueRO;

            SpatialUtility.QueryRadius(transform.Position, projectile.HitRadius, spatialIndex.CellSize, spatialIndex.Map, ref candidates);

            float radiusSq = projectile.HitRadius * projectile.HitRadius;

            for (int i = 0; i < candidates.Length; i++)
            {
                Entity candidate = candidates[i];

                if (candidate == projectile.OwnerEntity)
                    continue;

                if (!transformLookup.HasComponent(candidate) ||
                    !factionLookup.HasComponent(candidate) ||
                    !healthLookup.HasComponent(candidate))
                    continue;

                if (deadLookup.HasComponent(candidate))
                    continue;

                if (factionLookup[candidate].Value == projectile.OwnerFaction)
                    continue;

                float distanceSq = math.lengthsq(transformLookup[candidate].Position - transform.Position);
                if (distanceSq > radiusSq)
                    continue;

                if (ContainsRecentHit(recentHits, candidate))
                    continue;

                Entity damageEventEntity = ecb.CreateEntity();
                ecb.AddComponent(damageEventEntity, new DamageEventData
                {
                    Target = candidate,
                    Damage = projectile.Damage
                });

                recentHits.Add(new OrbitRecentHitData
                {
                    Target = candidate,
                    RemainingCooldown = projectile.HitCooldown
                });
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        candidates.Dispose();
    }

    private static bool ContainsRecentHit(DynamicBuffer<OrbitRecentHitData> recentHits, Entity candidate)
    {
        for (int i = 0; i < recentHits.Length; i++)
        {
            if (recentHits[i].Target == candidate)
                return true;
        }

        return false;
    }
}
