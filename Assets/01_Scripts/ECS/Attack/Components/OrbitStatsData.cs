using Unity.Entities;

public struct OrbitStatsData : IComponentData, IAddable<OrbitStatsData>, IInitializableStats<OrbitStatsData>
{
    public float DamageBonusRate;
    public float AttackSpeedBonusRate;
    public float OrbitSpeedBonusRate;
    public float OrbitRadiusBonus;
    public float HitRadiusBonus;
    public int ProjectileCountBonus;
    public float HitCooldownBonus;

    public OrbitStatsData Add(OrbitStatsData other)
    {
        return new OrbitStatsData
        {
            DamageBonusRate = DamageBonusRate + other.DamageBonusRate,
            AttackSpeedBonusRate = AttackSpeedBonusRate + other.AttackSpeedBonusRate,
            OrbitSpeedBonusRate = OrbitSpeedBonusRate + other.OrbitSpeedBonusRate,
            OrbitRadiusBonus = OrbitRadiusBonus + other.OrbitRadiusBonus,
            HitRadiusBonus = HitRadiusBonus + other.HitRadiusBonus,
            ProjectileCountBonus = ProjectileCountBonus + other.ProjectileCountBonus,
            HitCooldownBonus = HitCooldownBonus + other.HitCooldownBonus
        };
    }

    public void Initialize()
    {
        DamageBonusRate = 0f;
        AttackSpeedBonusRate = 0f;
        OrbitSpeedBonusRate = 0f;
        OrbitRadiusBonus = 0f;
        HitRadiusBonus = 0f;
        ProjectileCountBonus = 0;
        HitCooldownBonus = 0f;
    }
}
