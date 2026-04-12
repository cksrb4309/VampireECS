using Unity.Entities;

public struct BoomerangBaseStatsData : IComponentData
{
    public float BaseDamage;
    public float BaseAttackSpeed;
    public float BaseSpeed;
    public float BaseReturnSpeed;
    public float BaseReturnAcceleration;
    public float BaseMaxDistance;
    public float BaseHitRadius;
    public int BaseProjectileCount;
    public float BaseHitCooldown;
}
