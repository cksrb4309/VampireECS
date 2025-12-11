using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct RandomRotationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RandomRotationData>();
        state.RequireForUpdate<DynamicRotationData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;

        foreach (var (randomRW, dynamicRW) in
                 SystemAPI.Query<RefRW<RandomRotationData>, RefRW<DynamicRotationData>>())
        {
            ref var random = ref randomRW.ValueRW;
            ref var dynamic = ref dynamicRW.ValueRW;

            // 타이머 업데이트
            random.TimeUntilNextChange -= dt;
            if (random.TimeUntilNextChange <= 0f)
            {
                random.TimeUntilNextChange = UnityEngine.Random.Range(random.MinInterval, random.MaxInterval);
                random.TargetAxis = math.normalize(UnityEngine.Random.insideUnitSphere);
            }

            // 부드럽게 보간
            random.CurrentAxis = math.normalize(math.lerp(random.CurrentAxis, random.TargetAxis, dt * random.LerpSpeed));

            // dynamicRot에 축/속도만 전달 (여기서 회전 적용하지 않음)
            dynamic.RotationAxis = random.CurrentAxis;

            // 단위 일관성: RandomRotationData.RotationSpeed가 'degrees/sec'이라면 math.radians(...)
            // 여기서는 가정: random.RotationSpeed는 degrees/sec -> 변환
            dynamic.AngularSpeed = math.radians(random.AngularSpeed);
            // 만약 random.RotationSpeed가 이미 rad/sec라면 위 변환 없이 직접 할당
        }
    }
}