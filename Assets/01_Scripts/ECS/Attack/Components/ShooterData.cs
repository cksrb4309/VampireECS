using Unity.Entities;
using Unity.Mathematics;

public struct ShooterData : IComponentData, IAddable<ShooterData>
{
    public Entity ProjectilePrefab;

    public float3 Direction;
    public float3 MuzzleOffset;

    public Faction OwnerFaction;

    public float MuzzleDistance;

    // 마지막 발사 시간 기록
    public float ElapsedTime;

    public ShooterData Add(ShooterData other)
    {
        return this;
    }
}