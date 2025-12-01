using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerStats>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var
            (inputRO,
            statsRO,
            transformRW,
            velocityRW) in SystemAPI.Query<
                RefRO<PlayerInputData>,
                RefRO<PlayerStats>,
                RefRW<LocalTransform>,
                RefRW<PhysicsVelocity>>())
        {
            // 읽기 전용
            ref readonly var input = ref inputRO.ValueRO;
            ref readonly var stats = ref statsRO.ValueRO;

            // 읽기/쓰기 가능
            ref var transform = ref transformRW.ValueRW;
            ref var velocity = ref velocityRW.ValueRW;

            float3 dir = new float3(input.Move.x, 0f, input.Move.y);

            transform.Position += dir * stats.MoveSpeed * SystemAPI.Time.DeltaTime;

            velocity.Linear = float3.zero;
        }
    }
}
