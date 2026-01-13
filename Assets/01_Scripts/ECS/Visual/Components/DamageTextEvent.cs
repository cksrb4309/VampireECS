using Unity.Entities;
using Unity.Mathematics;

public struct DamageTextEvent : IComponentData
{
    public float3 WorldPosition;
    public int Damage;
    public float4 Color;
}
