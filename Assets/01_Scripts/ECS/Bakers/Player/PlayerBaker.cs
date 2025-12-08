using Unity.Entities;
using Unity.Mathematics;

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

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

        AddComponent(entity, new PlayerStats());

        AddComponent(entity, new PlayerInputData { Move = float2.zero });

        AddComponent(entity, new PlayerTag());  // 태그 컴포넌트도 추가 가능
    }
}
