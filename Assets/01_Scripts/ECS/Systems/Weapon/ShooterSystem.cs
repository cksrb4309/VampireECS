using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(DamagePreprocessSystemGroup))]
public partial struct ShooterSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var 
            (transformRO,
            shooterDataRW,
            canfireDataRO,
            attackDamageDataRO,
            projectileCountDataRO,
            projectileDurationDataRO,
            projectileSpeedDataRO) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<ShooterData>,
                RefRO<ShooterCanFireData>,
                RefRO<AttackDamageData>,
                RefRO<ProjectileCountData>,
                RefRO<ProjectileDurationData>,
                RefRO<ProjectileSpeedData>>())
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

            #region 투사체 발사

            int count = projectileCountDataRO.ValueRO.Count;
            float maxAngle = math.radians(90f);

            // 투사체 수에 따라 전체 퍼짐 각도를 점점 넓히되, 90도에 수렴
            float totalAngle = maxAngle * (1f - 1f / (count));

            // 투사체가 1개면 퍼짐 없음
            float angleStep = (count > 1) ? totalAngle / (count - 1) : 0f;

            // 시작 각도(왼쪽 끝)
            float startAngle = -totalAngle * 0.5f;

            for (int i = 0; i < count; i++)
            {
                Entity projectile = ecb.Instantiate(shooterData.ProjectilePrefab);

                float angle = startAngle + angleStep * i;

                // 회전 벡터 적용 (Y축 기준 회전)
                float3 rotatedDir = math.mul(quaternion.RotateY(angle), shooterData.Direction);

                float3 spawnPos =
                    transformRO.ValueRO.Position +
                    shooterData.MuzzleOffset +
                    rotatedDir * shooterData.MuzzleDistance;

                ecb.SetComponent(projectile, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = quaternion.LookRotationSafe(rotatedDir, math.up()),
                    Scale = 1f
                });

                ecb.SetComponent(projectile, new ProjectileData
                {
                    Direction = rotatedDir,
                    Speed = projectileSpeedDataRO.ValueRO.Speed,
                    Damage = attackDamageDataRO.ValueRO.Damage,
                    Duration = projectileDurationDataRO.ValueRO.Duration,
                    OwnerFaction = shooterData.OwnerFaction
                });
            }
            #endregion
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
