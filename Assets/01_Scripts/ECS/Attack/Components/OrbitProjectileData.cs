using Unity.Entities;

public struct OrbitProjectileData : IComponentData
{
    public Entity OwnerEntity;
    public Faction OwnerFaction;
    public float CurrentAngle;
    public float OrbitSpeed;
    public float OrbitRadius;
    public float Damage;
    public float HitRadius;
    public float HitCooldown;
    public float RemainingLifetime;
}
