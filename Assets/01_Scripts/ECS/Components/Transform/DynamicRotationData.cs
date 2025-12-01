using Unity.Entities;
using Unity.Mathematics;

public struct DynamicRotationData : IComponentData
{
    public float RotationSpeed;
    public float MinInterval;
    public float MaxInterval;
    public float LerpSpeed;

    public float3 CurrentAxis;
    public float3 TargetAxis;
    public float TimeUntilNextChange;
}
