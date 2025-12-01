using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ShrinkOverTimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShrinkOverTimeData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var 
            (shrinkRW,
            transformRW,
            entity) in SystemAPI.Query<
                RefRW<ShrinkOverTimeData>,
                RefRW<LocalTransform>>()
                .WithEntityAccess())
        {
            ref var shrink = ref shrinkRW.ValueRW;

            shrink.ElapsedTime += deltaTime;

            float t = shrink.ElapsedTime;

            // Lifetime 초과 시 삭제
            if (t >= shrink.Lifetime)
            {
                ecb.DestroyEntity(entity);
                continue;
            }

            // Keyframe 찾기
            int idx = 0;
            for (int i = 0; i < shrink.KeyTimes.Length - 1; i++)
            {
                if (t >= shrink.KeyTimes[i] && t <= shrink.KeyTimes[i + 1])
                {
                    idx = i;

                    break;
                }
            }

            // 현재 구간 비율 계산
            float localT = (t - shrink.KeyTimes[idx]) / (shrink.KeyTimes[idx + 1] - shrink.KeyTimes[idx]);

            // 선형 보간 (비선형 커브는 이 단계에서 Catmull-Rom 등 사용 가능)
            float scale = math.lerp(shrink.KeyScales[idx], shrink.KeyScales[idx + 1], localT);

            transformRW.ValueRW.Scale = scale;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}