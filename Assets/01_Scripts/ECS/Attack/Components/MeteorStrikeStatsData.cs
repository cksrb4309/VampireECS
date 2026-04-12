using Unity.Entities;

public struct MeteorStrikeStatsData : IComponentData, IAddable<MeteorStrikeStatsData>, IInitializableStats<MeteorStrikeStatsData>
{
    public float DamageBonusRate;
    public float AttackSpeedBonusRate;
    public float AcquireRadiusBonus;
    public float ImpactRadiusBonus;
    public float ImpactDelayBonus;
    public int MeteorCountBonus;
    public float ScatterRadiusBonus;

    public MeteorStrikeStatsData Add(MeteorStrikeStatsData other)
    {
        return new MeteorStrikeStatsData
        {
            DamageBonusRate = DamageBonusRate + other.DamageBonusRate,
            AttackSpeedBonusRate = AttackSpeedBonusRate + other.AttackSpeedBonusRate,
            AcquireRadiusBonus = AcquireRadiusBonus + other.AcquireRadiusBonus,
            ImpactRadiusBonus = ImpactRadiusBonus + other.ImpactRadiusBonus,
            ImpactDelayBonus = ImpactDelayBonus + other.ImpactDelayBonus,
            MeteorCountBonus = MeteorCountBonus + other.MeteorCountBonus,
            ScatterRadiusBonus = ScatterRadiusBonus + other.ScatterRadiusBonus
        };
    }

    public void Initialize()
    {
        DamageBonusRate = 0f;
        AttackSpeedBonusRate = 0f;
        AcquireRadiusBonus = 0f;
        ImpactRadiusBonus = 0f;
        ImpactDelayBonus = 0f;
        MeteorCountBonus = 0;
        ScatterRadiusBonus = 0f;
    }
}
