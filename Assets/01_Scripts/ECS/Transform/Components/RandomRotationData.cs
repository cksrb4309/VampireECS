using Unity.Entities;
using Unity.Mathematics;

public struct RandomRotationData : IComponentData
{
    public float AngularSpeed;
    public float MinInterval;
    public float MaxInterval;
    public float LerpSpeed;

    public float3 CurrentAxis;
    public float3 TargetAxis;
    public float TimeUntilNextChange;
}
