using Unity.Entities;

public struct ShooterBaseStatsData : IComponentData
{
    public float BaseDamage;
    public float BaseAttackSpeed;
    public float BaseProjectileSpeed;
    public float BaseProjectileDuration;
    public int BaseProjectileCount;
}
