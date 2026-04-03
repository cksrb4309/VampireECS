using Unity.Entities;

public struct BlackHoleStatsData : IComponentData, IAddable<BlackHoleStatsData>, IInitializableStats<BlackHoleStatsData>
{
    public float Damage;
    public float AttackSpeed;
    public float AcquireRadius;
    public float Radius;
    public float Duration;
    public float TickInterval;
    public float PullStrength;

    public BlackHoleStatsData Add(BlackHoleStatsData other)
    {
        return new BlackHoleStatsData
        {
            Damage = Damage + other.Damage,
            AttackSpeed = AttackSpeed + other.AttackSpeed,
            AcquireRadius = AcquireRadius + other.AcquireRadius,
            Radius = Radius + other.Radius,
            Duration = Duration + other.Duration,
            TickInterval = TickInterval + other.TickInterval,
            PullStrength = PullStrength + other.PullStrength
        };
    }

    public void Initialize()
    {
        Damage = 4f;
        AttackSpeed = 0.35f;
        AcquireRadius = 10f;
        Radius = 4f;
        Duration = 2.5f;
        TickInterval = 0.25f;
        PullStrength = 7.5f;
    }
}
