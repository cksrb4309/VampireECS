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
        // TODO: 기본 스탯 설정
        Damage = 1f;
        AttackSpeed = 1f;
        AttackRange = 1f;
    }
    public override string ToString()
    {
        return $"CombatStat Damage: {Damage}, AttackSpeed: {AttackSpeed}, AttackRange: {AttackRange}";
    }
}
