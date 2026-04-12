using Unity.Entities;

public struct OrbitBaseStatsData : IComponentData
{
    public float BaseDamage;
    public float BaseAttackSpeed;
    public float BaseOrbitSpeed;
    public float BaseOrbitRadius;
    public float BaseHitRadius;
    public int BaseProjectileCount;
    public float BaseHitCooldown;
}
