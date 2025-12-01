using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;

public class RotationBaker : Baker<RotationAuthoring>
{
    public override void Bake(RotationAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new RotationData
        {
            Axis = authoring.Axis,
            Speed = authoring.RotationSpeed
        });
    }
}
