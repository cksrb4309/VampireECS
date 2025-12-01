using Unity.Entities;
using Unity.Mathematics;

public class DynamicRotationBaker : Baker<DynamicRotationAuthoring>
{
    public override void Bake(DynamicRotationAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        float3 initialAxis = math.normalize(UnityEngine.Random.insideUnitSphere);

        float initialTimer =
            UnityEngine.Random.Range(authoring.MinDirectionChangeInterval,
                                     authoring.MaxDirectionChangeInterval);

        AddComponent(entity, new DynamicRotationData
        {
            RotationSpeed = authoring.RotationSpeed,
            MinInterval = authoring.MinDirectionChangeInterval,
            MaxInterval = authoring.MaxDirectionChangeInterval,
            LerpSpeed = authoring.DirectionLerpSpeed,

            CurrentAxis = initialAxis,
            TargetAxis = initialAxis,     // 처음에는 목표 축도 현재 축과 동일하게
            TimeUntilNextChange = initialTimer
        });
    }
}
