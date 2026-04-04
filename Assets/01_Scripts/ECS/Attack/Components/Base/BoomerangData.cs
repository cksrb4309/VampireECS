using Unity.Entities;
using Unity.Mathematics;

public struct BoomerangData : IComponentData, IAddable<BoomerangData>
{
    public Entity ProjectilePrefab;

    public float3 MuzzleOffset;

    public Faction OwnerFaction;

    public float MuzzleDistance;
    public float ElapsedTime;

    public BoomerangData Add(BoomerangData other)
    {
        return this;
    }
}
