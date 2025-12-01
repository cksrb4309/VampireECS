using Unity.Entities;
using Unity.Mathematics;

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent<PhysicsInit>(entity);

        AddComponent(entity, new PlayerStats { MoveSpeed = authoring.MoveSpeed });

        AddComponent(entity, new PlayerInputData { Move = float2.zero });

        AddComponent(entity, new PlayerTag());  // 태그 컴포넌트도 추가 가능
    }
}
