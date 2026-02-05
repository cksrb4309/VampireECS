using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(DamageSetupSystemGroup))]
public partial struct PlayerMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float timeScale = SystemAPI.GetSingleton<GameTimeScale>().Value;

        foreach (var
            (inputRO,
            moveDataRW,
            transformRW,
            velocityRW) in SystemAPI.Query<
                RefRO<PlayerInputData>,
                RefRW<PlayerMoveData>,
                RefRW<LocalTransform>,
                RefRW<PhysicsVelocity>>())
        {
            ref readonly PlayerInputData input = ref inputRO.ValueRO;
            ref PlayerMoveData moveData        = ref moveDataRW.ValueRW;
            ref LocalTransform transform       = ref transformRW.ValueRW;
            ref PhysicsVelocity velocity       = ref velocityRW.ValueRW;

            float3 targetDir = math.normalize(new float3(input.Move.x, 0f, input.Move.y));

            if (input.Move.x == 0f && input.Move.y == 0f)
            {
                float3 decelerationDelta = math.normalize(moveData.Velocity) * (moveData.Deceleration * deltaTime);

                if (math.length(moveData.Velocity) > math.length(decelerationDelta))
                {
                    moveData.Velocity = moveData.Velocity - math.normalize(moveData.Velocity) * (moveData.Deceleration * deltaTime);
                }
            }
            else
            {
                moveData.Velocity = moveData.Velocity + targetDir * (moveData.Acceleration * deltaTime);
            }


            if (math.length(moveData.Velocity) > moveData.MaxSpeed)
            {
                moveData.Velocity = math.normalize(moveData.Velocity) * moveData.MaxSpeed;
            }

            transform.Position += moveData.Velocity * SystemAPI.Time.DeltaTime * timeScale;

            velocity.Linear = float3.zero;
        }
    }
}
