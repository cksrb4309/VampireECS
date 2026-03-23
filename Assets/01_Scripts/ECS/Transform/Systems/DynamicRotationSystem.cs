using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MovementRotationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DynamicRotationData>();
    }
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;

        foreach (var (dynamicRW, transformRW) in
                 SystemAPI.Query<RefRW<DynamicRotationData>, RefRW<LocalTransform>>())
        {
            ref var dynamic = ref dynamicRW.ValueRW;
            ref var lt = ref transformRW.ValueRW;

            // 안전성: 축이 유효하고 속도가 0이 아니면 누적
            if (math.lengthsq(dynamic.RotationAxis) > 0f && math.abs(dynamic.AngularSpeed) > 0f)
            {
                float3 axis = math.normalize(dynamic.RotationAxis);
                quaternion delta = quaternion.AxisAngle(axis, dynamic.AngularSpeed * dt);

                // 누적: AccumulatedRotation = AccumulatedRotation * delta
                // (오른쪽에 덧붙이는 방식 — BaseRotation * AccumulatedRotation가 최종)
                dynamic.AccumulatedRotation = math.mul(dynamic.AccumulatedRotation, delta);
            }

            // 최종 회전: BaseRotation * AccumulatedRotation
            lt.Rotation = math.mul(dynamic.BaseRotation, dynamic.AccumulatedRotation);
        }
    }
}
