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
        in ShooterBaseStatsData shooterBaseStatsData,
        in ShooterStatsData shooterStatsData,
        in CombatStatsData combatStatsData)
    {
        if (!canfireData.CanFire) return;

        float finalAttackSpeed =
            shooterBaseStatsData.BaseAttackSpeed *
            (1f + shooterStatsData.AttackSpeedBonusRate) *
            combatStatsData.AttackSpeed;

        shooterData.ElapsedTime += DeltaTime * finalAttackSpeed;

        if (shooterData.ElapsedTime < 1f) return;

        #region 투사체 발사

        int count = math.max(1, shooterBaseStatsData.BaseProjectileCount + shooterStatsData.ProjectileCountBonus);
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
                    Speed =
                        shooterBaseStatsData.BaseProjectileSpeed *
                        (1f + shooterStatsData.ProjectileSpeedBonusRate),
                    Damage =
                        shooterBaseStatsData.BaseDamage *
                        (1f + shooterStatsData.DamageBonusRate) *
                        combatStatsData.Damage,
                    Duration = math.max(0.05f, shooterBaseStatsData.BaseProjectileDuration + shooterStatsData.ProjectileDurationBonus),
                    OwnerFaction = shooterData.OwnerFaction
                });
            }
        }

        #endregion
    }
}
