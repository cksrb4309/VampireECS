using Unity.Entities;

public struct AuraStatsData : IComponentData, IAddable<AuraStatsData>, IInitializableStats<AuraStatsData>
{
    public float Damage;
    public float AttackSpeed;
    public float Radius;

    public AuraStatsData Add(AuraStatsData other)
    {
        return new AuraStatsData
        {
            Damage = this.Damage + other.Damage,
            AttackSpeed = this.AttackSpeed + other.AttackSpeed,
            Radius = this.Radius + other.Radius
        };
    }
    public void Initialize()
    {
        Damage = 10f;
        AttackSpeed = 1f;
        Radius = 10f;
    }
    public override string ToString()
    {
        return $"AuraStatsData Damage: {Damage}, AttackSpeed: {AttackSpeed}, Radius: {Radius}";
    }
}
