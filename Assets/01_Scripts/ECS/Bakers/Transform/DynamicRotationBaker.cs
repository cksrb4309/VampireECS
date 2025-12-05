using Unity.Entities;
using Unity.Mathematics;

public class DynamicRotationBaker : Baker<DynamicRotationAuthoring>
{
    public override void Bake(DynamicRotationAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new DynamicRotationData
        {
            AngularSpeed = 0,
            RotationAxis = 0,
            BaseRotation = authoring.transform.rotation,
            AccumulatedRotation = quaternion.identity
        });
    }
}