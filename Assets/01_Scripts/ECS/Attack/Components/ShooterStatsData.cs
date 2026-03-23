using Unity.Entities;

public struct ShooterStatsData : IComponentData, IAddable<ShooterStatsData>, IInitializableStats<ShooterStatsData>
{
    public float Damage;
    public float AttackSpeed;
    public float ProjectileSpeed;
    public float ProjectileDuration;
    public int ProjectileCount;

    public ShooterStatsData Add(ShooterStatsData other)
    {
        return new ShooterStatsData
        {
            Damage = this.Damage + other.Damage,
            AttackSpeed = this.AttackSpeed + other.AttackSpeed,
            ProjectileSpeed = this.ProjectileSpeed + other.ProjectileSpeed,
            ProjectileDuration = this.ProjectileDuration + other.ProjectileDuration,
            ProjectileCount = this.ProjectileCount + other.ProjectileCount
        };
    }
    public void Initialize()
    {
        Damage = 20f;
        AttackSpeed = 5f;
        ProjectileSpeed = 20f;
        ProjectileDuration = 1f;
        ProjectileCount = 1;
    }
}
