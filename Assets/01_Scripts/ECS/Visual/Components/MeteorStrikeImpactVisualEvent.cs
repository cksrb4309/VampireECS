using Unity.Entities;
using Unity.Mathematics;

public struct MeteorStrikeImpactVisualEvent : IComponentData
{
    public float3 Position;
    public float Radius;
    public float Duration;
    public float StrikeHeight;
}
