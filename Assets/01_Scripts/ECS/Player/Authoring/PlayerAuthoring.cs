using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float MaxSpeed = 30f;
    public float Acceleration = 15f;
    public float Deceleration = 15f;
    public float RotationSpeed = 360f;
    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new SpatialCell { Value = new int2 { x = 1000, y = 1000 } });

            AddComponent(entity, new FactionData { Value = Faction.Player });

            AddComponent<LockYToZero>(entity);

            AddComponent<PhysicsInit>(entity);

            AddComponent(entity, new PlayerMoveData
            {
                MaxSpeed = authoring.MaxSpeed,
                Acceleration = authoring.Acceleration,
                Deceleration = authoring.Deceleration,
                AngularSpeed = authoring.RotationSpeed,
                Velocity = new float3(0, 0, 0.001f)
            });

            AddComponent(entity, new PlayerExpData
            {
                Level = 1,
                Required = 10,
                Current = 0
            });

            AddComponent(entity, new PlayerInputData { Move = float2.zero });

            AddComponent(entity, new PlayerTag());  // 태그 컴포넌트도 추가 가능
        }
    }
}
