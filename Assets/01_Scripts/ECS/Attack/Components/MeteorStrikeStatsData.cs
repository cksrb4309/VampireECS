using Unity.Entities;

public struct MeteorStrikeStatsData : IComponentData, IAddable<MeteorStrikeStatsData>, IInitializableStats<MeteorStrikeStatsData>
{
    public float Damage;
    public float AttackSpeed;
    public float AcquireRadius;
    public float ImpactRadius;
    public float ImpactDelay;
    public int MeteorCount;
    public float ScatterRadius;

    public MeteorStrikeStatsData Add(MeteorStrikeStatsData other)
    {
        return new MeteorStrikeStatsData
        {
            Damage = Damage + other.Damage,
            AttackSpeed = AttackSpeed + other.AttackSpeed,
            AcquireRadius = AcquireRadius + other.AcquireRadius,
            ImpactRadius = ImpactRadius + other.ImpactRadius,
            ImpactDelay = ImpactDelay + other.ImpactDelay,
            MeteorCount = MeteorCount + other.MeteorCount,
            ScatterRadius = ScatterRadius + other.ScatterRadius
        };
    }

    public void Initialize()
    {
        Damage = 45f;
        AttackSpeed = 0.4f;
        AcquireRadius = 10f;
        ImpactRadius = 2.5f;
        ImpactDelay = 0.8f;
        MeteorCount = 1;
        ScatterRadius = 1.5f;
    }
}
