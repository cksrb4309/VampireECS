using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ChainLightningSegment
{
    public float3 From;
    public float3 To;
}

public struct ChainLightningVisualEvent : IComponentData
{
    public FixedList512Bytes<ChainLightningSegment> Segments;
    public float Duration;
}
