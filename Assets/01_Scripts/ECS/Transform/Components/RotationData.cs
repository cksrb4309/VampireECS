using Unity.Entities;
using Unity.Mathematics;

public struct RotationData : IComponentData
{
    public float3 Axis;
    public float Speed;
}