using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct RenderRotationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DynamicRotationData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var
            (rotationRW,
            transformRW)
                 in SystemAPI.Query<
                     RefRW<DynamicRotationData>,
                     RefRW<LocalTransform>>())
        {
            // 읽기/쓰기 가능한 데이터
            ref var data = ref rotationRW.ValueRW;
            ref var lt = ref transformRW.ValueRW;

            // 다음 방향 전환까지의 시간 감소
            data.TimeUntilNextChange -= deltaTime;

            // 시간이 다 되면 새로운 방향 선택
            if (data.TimeUntilNextChange <= 0f)
            {
                data.TimeUntilNextChange =
                    UnityEngine.Random.Range(data.MinInterval, data.MaxInterval);

                // 새로운 방향: 정규화된 랜덤 회전축
                data.TargetAxis = math.normalize(UnityEngine.Random.insideUnitSphere);
            }

            // 현재 회전 방향 → 목표 방향 보간
            data.CurrentAxis = math.normalize(
                math.lerp(data.CurrentAxis, data.TargetAxis, deltaTime * data.LerpSpeed)
            );

            // 회전 속도 적용
            float angle = data.RotationSpeed * deltaTime;
            quaternion rot = quaternion.AxisAngle(data.CurrentAxis, math.radians(angle));

            lt.Rotation = math.mul(rot, lt.Rotation);
        }
    }
}