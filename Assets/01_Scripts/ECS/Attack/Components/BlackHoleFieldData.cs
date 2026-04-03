using Unity.Entities;
using Unity.Mathematics;

public struct BlackHoleFieldData : IComponentData
{
    public float3 Position;
    public float Radius;
    public float PullStrength;
    public float TickDamage;
    public float TickInterval;
    public float TickElapsedTime;
    public float TotalDuration;
    public float RemainingDuration;
    public Faction OwnerFaction;
}
