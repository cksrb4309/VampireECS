using Unity.Entities;
public struct CombatStatsData : IComponentData, IAddable<CombatStatsData>, IInitializableStats<CombatStatsData>
{
    public float Damage;
    public float AttackSpeed;
    public float AttackRange;

    public CombatStatsData Add(CombatStatsData other)
    {
        return new CombatStatsData
        {
            Damage = this.Damage + other.Damage,
            AttackSpeed = this.AttackSpeed + other.AttackSpeed,
            AttackRange = this.AttackRange + other.AttackRange
        };
    }
    public void Initialize()
    {
        Damage = 1234f;
        AttackSpeed = 1f;
        AttackRange = 1f;
    }
    public override string ToString()
    {
        return $"AuraStatsData Damage: {Damage}, AttackSpeed: {AttackSpeed}, AttackRange: {AttackRange}";
    }
}
