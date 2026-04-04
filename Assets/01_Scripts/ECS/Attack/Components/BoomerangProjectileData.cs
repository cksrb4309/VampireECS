using Unity.Entities;
using Unity.Mathematics;

public struct BoomerangProjectileData : IComponentData
{
    public Entity OwnerEntity;
    public Faction OwnerFaction;
    public float3 Direction;
    public float Speed;
    public float ReturnSpeed;
    public float Damage;
    public float MaxDistance;
    public float HitRadius;
    public float HitCooldown;
    public float DistanceTraveled;
    public bool IsReturning;
}
