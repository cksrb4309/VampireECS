using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(BoomerangCastSystem))]
public partial struct BoomerangMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoomerangProjectileData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;
        ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transformRW, projectileRW, recentHits, boomerangEntity) in
            SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<BoomerangProjectileData>,
                DynamicBuffer<BoomerangRecentHitData>>()
            .WithEntityAccess())
        {
            ref LocalTransform transform = ref transformRW.ValueRW;
            ref BoomerangProjectileData projectile = ref projectileRW.ValueRW;

            TickRecentHits(deltaTime, recentHits);

            if (projectile.OwnerEntity == Entity.Null || !transformLookup.HasComponent(projectile.OwnerEntity))
            {
                ecb.DestroyEntity(boomerangEntity);
                continue;
            }

            if (projectile.IsReturning)
            {
                float3 ownerPosition = transformLookup[projectile.OwnerEntity].Position;
                float3 toOwner = ownerPosition - transform.Position;
                float distanceToOwner = math.length(toOwner);
                float moveDistance = projectile.ReturnSpeed * deltaTime;
                float catchDistance = math.max(0.5f, projectile.HitRadius);

                if (distanceToOwner <= math.max(catchDistance, moveDistance))
                {
                    ecb.DestroyEntity(boomerangEntity);
                    continue;
                }

                float3 returnDirection = math.normalize(toOwner);
                transform.Position += returnDirection * moveDistance;
                transform.Rotation = quaternion.LookRotationSafe(returnDirection, math.up());
                projectile.Direction = returnDirection;
            }
            else
            {
                float moveDistance = projectile.Speed * deltaTime;
                transform.Position += projectile.Direction * moveDistance;
                transform.Rotation = quaternion.LookRotationSafe(projectile.Direction, math.up());
                projectile.DistanceTraveled += moveDistance;

                if (projectile.DistanceTraveled >= projectile.MaxDistance)
                {
                    projectile.IsReturning = true;
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private static void TickRecentHits(float deltaTime, DynamicBuffer<BoomerangRecentHitData> recentHits)
    {
        for (int i = recentHits.Length - 1; i >= 0; i--)
        {
            BoomerangRecentHitData recentHit = recentHits[i];
            recentHit.RemainingCooldown -= deltaTime;

            if (recentHit.RemainingCooldown <= 0f)
            {
                recentHits.RemoveAt(i);
                continue;
            }

            recentHits[i] = recentHit;
        }
    }
}
