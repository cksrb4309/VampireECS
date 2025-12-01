using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct EnemyFollowSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // 플레이어 위치
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        float3 playerPos = SystemAPI
            .GetComponent<LocalTransform>(playerEntity)
            .Position;

        foreach (var 
            (transformRO,
            velocityRW,
            moveDataRO) in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<PhysicsVelocity>,
                RefRO<EnemyMoveData>>())
        {
            ref readonly var transform = ref transformRO.ValueRO;
            ref readonly var moveData = ref moveDataRO.ValueRO;
            ref var velocity = ref velocityRW.ValueRW;

            float3 dir = playerPos - transform.Position;

            dir = math.normalize(dir);

            velocity.Linear = dir * moveData.Speed;
        }
    }
}