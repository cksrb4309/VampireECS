using Unity.Entities;

public struct BlackHoleStatsData : IComponentData, IAddable<BlackHoleStatsData>, IInitializableStats<BlackHoleStatsData>
{
    public float DamageBonusRate;
    public float AttackSpeedBonusRate;
    public float AcquireRadiusBonus;
    public float RadiusBonus;
    public float DurationBonus;
    public float TickIntervalBonus;
    public float PullStrengthBonus;

    public BlackHoleStatsData Add(BlackHoleStatsData other)
    {
        return new BlackHoleStatsData
        {
            DamageBonusRate = DamageBonusRate + other.DamageBonusRate,
            AttackSpeedBonusRate = AttackSpeedBonusRate + other.AttackSpeedBonusRate,
            AcquireRadiusBonus = AcquireRadiusBonus + other.AcquireRadiusBonus,
            RadiusBonus = RadiusBonus + other.RadiusBonus,
            DurationBonus = DurationBonus + other.DurationBonus,
            TickIntervalBonus = TickIntervalBonus + other.TickIntervalBonus,
            PullStrengthBonus = PullStrengthBonus + other.PullStrengthBonus
        };
    }

    public void Initialize()
    {
        DamageBonusRate = 0f;
        AttackSpeedBonusRate = 0f;
        AcquireRadiusBonus = 0f;
        RadiusBonus = 0f;
        DurationBonus = 0f;
        TickIntervalBonus = 0f;
        PullStrengthBonus = 0f;
    }
}
