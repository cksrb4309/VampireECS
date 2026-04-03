using Unity.Entities;
using Unity.Mathematics;

public struct MeteorStrikeTelegraphVisualEvent : IComponentData
{
    public float3 Position;
    public float Radius;
    public float Duration;
}
