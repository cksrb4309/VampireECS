using Unity.Entities;
using Unity.Mathematics;

public struct ShooterData : IComponentData
{
    public Entity ProjectilePrefab;

    public float3 Direction;
    public float3 MuzzleOffset;

    public Faction OwnerFaction;

    public float MuzzleDistance;
    public float FireRate;

    // 마지막 발사 시간 기록
    public float TimeSinceLastFire;
}