using Unity.Entities;
using Unity.Mathematics;

public struct OrbitData : IComponentData, IAddable<OrbitData>
{
    public Entity ProjectilePrefab;
    public Faction OwnerFaction;
    public float ElapsedTime;

    public OrbitData Add(OrbitData other)
    {
        return this;
    }
}
