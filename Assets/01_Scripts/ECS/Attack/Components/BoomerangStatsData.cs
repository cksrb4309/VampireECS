using Unity.Entities;

public struct BoomerangStatsData : IComponentData, IAddable<BoomerangStatsData>, IInitializableStats<BoomerangStatsData>
{
    public float DamageBonusRate;
    public float AttackSpeedBonusRate;
    public float SpeedBonusRate;
    public float ReturnSpeedBonusRate;
    public float MaxDistanceBonus;
    public float HitRadiusBonus;
    public int ProjectileCountBonus;
    public float HitCooldownBonus;

    public BoomerangStatsData Add(BoomerangStatsData other)
    {
        return new BoomerangStatsData
        {
            DamageBonusRate = DamageBonusRate + other.DamageBonusRate,
            AttackSpeedBonusRate = AttackSpeedBonusRate + other.AttackSpeedBonusRate,
            SpeedBonusRate = SpeedBonusRate + other.SpeedBonusRate,
            ReturnSpeedBonusRate = ReturnSpeedBonusRate + other.ReturnSpeedBonusRate,
            MaxDistanceBonus = MaxDistanceBonus + other.MaxDistanceBonus,
            HitRadiusBonus = HitRadiusBonus + other.HitRadiusBonus,
            ProjectileCountBonus = ProjectileCountBonus + other.ProjectileCountBonus,
            HitCooldownBonus = HitCooldownBonus + other.HitCooldownBonus
        };
    }

    public void Initialize()
    {
        DamageBonusRate = 0f;
        AttackSpeedBonusRate = 0f;
        SpeedBonusRate = 0f;
        ReturnSpeedBonusRate = 0f;
        MaxDistanceBonus = 0f;
        HitRadiusBonus = 0f;
        ProjectileCountBonus = 0;
        HitCooldownBonus = 0f;
    }
}
