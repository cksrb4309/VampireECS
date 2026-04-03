using Unity.Entities;
using Unity.Mathematics;

public struct MeteorStrikePendingData : IComponentData
{
    public float3 Position;
    public float Damage;
    public float Radius;
    public float RemainingDelay;
    public Faction OwnerFaction;
}
