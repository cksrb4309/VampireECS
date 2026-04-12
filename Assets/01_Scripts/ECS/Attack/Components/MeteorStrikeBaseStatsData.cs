using Unity.Entities;

public struct MeteorStrikeBaseStatsData : IComponentData
{
    public float BaseDamage;
    public float BaseAttackSpeed;
    public float BaseAcquireRadius;
    public float BaseImpactRadius;
    public float BaseImpactDelay;
    public int BaseMeteorCount;
    public float BaseScatterRadius;
}
