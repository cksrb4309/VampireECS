using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerRotationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerMoveData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (moveDataRO, dynamicRotRW) in
                 SystemAPI.Query<
                     RefRO<PlayerMoveData>,
                     RefRW<DynamicRotationData>>())
        {
            ref var dynamicRot = ref dynamicRotRW.ValueRW;

            float3 velocity = moveDataRO.ValueRO.Velocity;
            dynamicRot.AngularSpeed = moveDataRO.ValueRO.AngularSpeed;

            if (math.lengthsq(velocity) <= 0f)
                return;

            float3 forward = math.normalize(velocity);

            dynamicRot.BaseRotation =
                quaternion.LookRotationSafe(forward, math.up());

            // ✅ 로컬 기준 forward 축 (드릴 회전용)
            dynamicRot.RotationAxis = new float3(0, 0, 1);
        }
    }
}
