using Unity.Entities;

public struct ChainLightningStatsData : IComponentData, IAddable<ChainLightningStatsData>, IInitializableStats<ChainLightningStatsData>
{
    public float Damage;
    public float AttackSpeed;
    public float AcquireRadius;
    public float JumpRadius;
    public int MaxTargets;
    public float DamageMultiplierPerJump;

    public ChainLightningStatsData Add(ChainLightningStatsData other)
    {
        return new ChainLightningStatsData
        {
            Damage = Damage + other.Damage,
            AttackSpeed = AttackSpeed + other.AttackSpeed,
            AcquireRadius = AcquireRadius + other.AcquireRadius,
            JumpRadius = JumpRadius + other.JumpRadius,
            MaxTargets = MaxTargets + other.MaxTargets,
            DamageMultiplierPerJump = DamageMultiplierPerJump + other.DamageMultiplierPerJump
        };
    }

    public void Initialize()
    {
        Damage = 15f;
        AttackSpeed = 1f;
        AcquireRadius = 8f;
        JumpRadius = 5f;
        MaxTargets = 3;
        DamageMultiplierPerJump = 0.85f;
    }
}
