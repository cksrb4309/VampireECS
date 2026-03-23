using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct ProjectileSpawnJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute(
        [EntityIndexInQuery] int index,
        in LocalTransform transform,
        ref ShooterData shooterData,
        in ShooterCanFireData canfireData,
        in ShooterStatsData shooterStatsData,
        in CombatStatsData combatStatsData)
    {
        if (!canfireData.CanFire) return;

        shooterData.ElapsedTime += DeltaTime * shooterStatsData.AttackSpeed * combatStatsData.AttackSpeed;

        if (shooterData.ElapsedTime < 1f) return;

        #region 투사체 발사

        int count = shooterStatsData.ProjectileCount;
        float maxAngle = math.radians(90f);

        // 투사체 수에 따라 전체 퍼짐 각도를 점점 넓히되, 90도에 수렴
        float totalAngle = maxAngle * (1f - 1f / (count));

        // 투사체가 1개면 퍼짐 없음
        float angleStep = (count > 1) ? totalAngle / (count - 1) : 0f;

        // 시작 각도(왼쪽 끝)
        float startAngle = -totalAngle * 0.5f;

        while (shooterData.ElapsedTime >= 1f)
        {
            shooterData.ElapsedTime -= 1f;

            for (int i = 0; i < count; i++)
            {
                Entity projectile = ECB.Instantiate(index, shooterData.ProjectilePrefab);

                float angle = startAngle + angleStep * i;

                // 회전 벡터 적용 (Y축 기준 회전)
                float3 rotatedDir = math.mul(quaternion.RotateY(angle), shooterData.Direction);

                float3 spawnPos =
                    transform.Position +
                    shooterData.MuzzleOffset +
                    rotatedDir * shooterData.MuzzleDistance;

                ECB.SetComponent(index, projectile, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = quaternion.LookRotationSafe(rotatedDir, math.up()),
                    Scale = 1f
                });

                ECB.SetComponent(index, projectile, new ProjectileData
                {
                    Direction = rotatedDir,
                    Speed = shooterStatsData.ProjectileSpeed,
                    Damage = shooterStatsData.Damage * combatStatsData.Damage,
                    Duration = shooterStatsData.ProjectileDuration,
                    OwnerFaction = shooterData.OwnerFaction
                });
            }
        }

        #endregion
    }
}
