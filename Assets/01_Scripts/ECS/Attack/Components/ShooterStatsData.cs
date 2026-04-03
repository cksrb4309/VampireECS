using Unity.Entities;

public struct ShooterStatsData : IComponentData, IAddable<ShooterStatsData>, IInitializableStats<ShooterStatsData>
{
    public float DamageBonusRate;
    public float AttackSpeedBonusRate;
    public float ProjectileSpeedBonusRate;
    public float ProjectileDurationBonus;
    public int ProjectileCountBonus;

    public ShooterStatsData Add(ShooterStatsData other)
    {
        return new ShooterStatsData
        {
            DamageBonusRate = DamageBonusRate + other.DamageBonusRate,
            AttackSpeedBonusRate = AttackSpeedBonusRate + other.AttackSpeedBonusRate,
            ProjectileSpeedBonusRate = ProjectileSpeedBonusRate + other.ProjectileSpeedBonusRate,
            ProjectileDurationBonus = ProjectileDurationBonus + other.ProjectileDurationBonus,
            ProjectileCountBonus = ProjectileCountBonus + other.ProjectileCountBonus
        };
    }
    public void Initialize()
    {
        DamageBonusRate = 0f;
        AttackSpeedBonusRate = 0f;
        ProjectileSpeedBonusRate = 0f;
        ProjectileDurationBonus = 0f;
        ProjectileCountBonus = 0;
    }
}
