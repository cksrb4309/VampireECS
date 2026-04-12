using Unity.Entities;

public struct ChainLightningStatsData : IComponentData, IAddable<ChainLightningStatsData>, IInitializableStats<ChainLightningStatsData>
{
    public float DamageBonusRate;
    public float AttackSpeedBonusRate;
    public float AcquireRadiusBonus;
    public float JumpRadiusBonus;
    public int MaxTargetsBonus;
    public float DamageMultiplierPerJumpBonus;

    public ChainLightningStatsData Add(ChainLightningStatsData other)
    {
        return new ChainLightningStatsData
        {
            DamageBonusRate = DamageBonusRate + other.DamageBonusRate,
            AttackSpeedBonusRate = AttackSpeedBonusRate + other.AttackSpeedBonusRate,
            AcquireRadiusBonus = AcquireRadiusBonus + other.AcquireRadiusBonus,
            JumpRadiusBonus = JumpRadiusBonus + other.JumpRadiusBonus,
            MaxTargetsBonus = MaxTargetsBonus + other.MaxTargetsBonus,
            DamageMultiplierPerJumpBonus = DamageMultiplierPerJumpBonus + other.DamageMultiplierPerJumpBonus
        };
    }

    public void Initialize()
    {
        DamageBonusRate = 0f;
        AttackSpeedBonusRate = 0f;
        AcquireRadiusBonus = 0f;
        JumpRadiusBonus = 0f;
        MaxTargetsBonus = 0;
        DamageMultiplierPerJumpBonus = 0f;
    }
}
