using Unity.Entities;

public struct AuraStatsData : IComponentData, IAddable<AuraStatsData>, IInitializableStats<AuraStatsData>
{
    public float DamageBonusRate;
    public float AttackSpeedBonusRate;
    public float RadiusBonusRate;

    public AuraStatsData Add(AuraStatsData other)
    {
        return new AuraStatsData
        {
            DamageBonusRate = DamageBonusRate + other.DamageBonusRate,
            AttackSpeedBonusRate = AttackSpeedBonusRate + other.AttackSpeedBonusRate,
            RadiusBonusRate = RadiusBonusRate + other.RadiusBonusRate
        };
    }

    public void Initialize()
    {
        DamageBonusRate = 0f;
        AttackSpeedBonusRate = 0f;
        RadiusBonusRate = 0f;
    }

    public override string ToString()
    {
        return $"AuraStatsData DamageBonusRate: {DamageBonusRate}, AttackSpeedBonusRate: {AttackSpeedBonusRate}, RadiusBonusRate: {RadiusBonusRate}";
    }
}
