using Unity.Entities;
using UnityEngine;

public class RotationAuthoring : MonoBehaviour
{
    public Vector3 Axis = Vector3.up;
    public float RotationSpeed = 180f;
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
}
