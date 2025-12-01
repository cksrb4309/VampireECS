using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct ShooterSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (transformRO, shooterDataRW, canfireDataRO, entity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<ShooterData>,
                RefRO<ShooterCanFireData>>()
                     .WithEntityAccess())
        {
            ref readonly var transform = ref transformRO.ValueRO;
            ref var shooterData = ref shooterDataRW.ValueRW;

            shooterData.TimeSinceLastFire += deltaTime;

            if (shooterData.TimeSinceLastFire < shooterData.FireRate)
            {
                continue;
            }
            if (!canfireDataRO.ValueRO.CanFire)
            {
                continue;
            }

            shooterData.TimeSinceLastFire = 0;
            Entity projectile = ecb.Instantiate(shooterData.ProjectilePrefab);

            float3 spawnPos =
                transform.Position +
                shooterData.MuzzleOffset +
                shooterData.Direction * shooterData.MuzzleDistance;

            ecb.SetComponent(projectile, new LocalTransform
            {
                Position = spawnPos,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            ecb.SetComponent(projectile, new ProjectileData
            {
                Direction = shooterData.Direction,
                Speed = shooterData.Speed,
                Damage = shooterData.Damage,
                Lifetime = shooterData.Lifetime,
                OwnerFaction = shooterData.OwnerFaction
            });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    [BurstCompile]
    private void SpawnProjectile(ref SystemState state, ref ShooterData shooter, Entity ownerEntity, ref EntityCommandBuffer ecb)
    {
        float3 spawnPos = GetShooterWorldPosition(ref state, ownerEntity) + shooter.MuzzleOffset + (shooter.Direction * shooter.MuzzleDistance);

        Entity projectileEntity = ecb.Instantiate(shooter.ProjectilePrefab);

        ecb.SetComponent(projectileEntity, new LocalTransform
        {
            Position = spawnPos,
            Scale = 1f,
            Rotation = quaternion.identity
        });

        ecb.AddComponent(projectileEntity, new ProjectileData
        {
            Direction = shooter.Direction,
            Speed = shooter.Speed,
            Damage = shooter.Damage,
            Lifetime = shooter.Lifetime,
            OwnerFaction = shooter.OwnerFaction
        });
    }
    [BurstCompile]
    private float3 GetShooterWorldPosition(ref SystemState state, Entity shooter)
    {
        // LocalTransform을 가져와서 월드 위치 계산
        return SystemAPI.GetComponent<LocalToWorld>(shooter).Position;
    }
}
