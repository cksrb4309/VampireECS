using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(OrbitCastSystem))]
public partial struct OrbitMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OrbitProjectileData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;
        ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transformRW, projectileRW, recentHits, orbitEntity) in
            SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<OrbitProjectileData>,
                DynamicBuffer<OrbitRecentHitData>>()
            .WithEntityAccess())
        {
            ref LocalTransform transform = ref transformRW.ValueRW;
            ref OrbitProjectileData projectile = ref projectileRW.ValueRW;

            projectile.RemainingLifetime -= deltaTime;
            if (projectile.RemainingLifetime <= 0f)
            {
                ecb.DestroyEntity(orbitEntity);
                continue;
            }

            if (projectile.OwnerEntity == Entity.Null || !transformLookup.HasComponent(projectile.OwnerEntity))
            {
                ecb.DestroyEntity(orbitEntity);
                continue;
            }

            TickRecentHits(deltaTime, recentHits);

            projectile.CurrentAngle += projectile.OrbitSpeed * deltaTime;

            float3 ownerPosition = transformLookup[projectile.OwnerEntity].Position;
            float3 offset = new float3(math.cos(projectile.CurrentAngle), 0f, math.sin(projectile.CurrentAngle)) * projectile.OrbitRadius;

            transform.Position = ownerPosition + offset;
            transform.Rotation = GetOrbitRotation(projectile.CurrentAngle);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private static void TickRecentHits(float deltaTime, DynamicBuffer<OrbitRecentHitData> recentHits)
    {
        for (int i = recentHits.Length - 1; i >= 0; i--)
        {
            OrbitRecentHitData entry = recentHits[i];
            entry.RemainingCooldown -= deltaTime;

            if (entry.RemainingCooldown <= 0f)
            {
                recentHits.RemoveAt(i);
                continue;
            }

            recentHits[i] = entry;
        }
    }

    private static quaternion GetOrbitRotation(float angle)
    {
        // Face the tangential direction (90° offset from radial direction)
        float tangentAngle = angle + math.PI * 0.5f;
        float3 tangent = new float3(math.cos(tangentAngle), 0f, math.sin(tangentAngle));
        return quaternion.LookRotationSafe(tangent, math.up());
    }
}
