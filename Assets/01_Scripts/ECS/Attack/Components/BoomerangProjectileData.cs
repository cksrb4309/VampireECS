using Unity.Entities;
using Unity.Mathematics;

public struct BoomerangProjectileData : IComponentData
{
    public Entity OwnerEntity;
    public Faction OwnerFaction;
    public float3 Direction;
    public float Speed;
    public float ReturnSpeed;
    public float ReturnAcceleration;
    public float CurrentReturnSpeed;
    public float Damage;
    public float MaxDistance;
    public float HitRadius;
    public float HitCooldown;
    public float DistanceTraveled;
    public float SpinAngle;
    public float SpinSpeed;
    public bool IsReturning;
}
