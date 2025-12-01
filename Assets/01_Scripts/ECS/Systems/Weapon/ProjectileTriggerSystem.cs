using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public partial struct ProjectileTriggerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // 단일 ECB 생성
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        // SimulationSingleton 가져오기
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        // TriggerJob 생성
        var job = new ProjectileTriggerJob
        {
            HasHealthLookup = SystemAPI.GetComponentLookup<HealthData>(true),
            ProjectileLookup = SystemAPI.GetComponentLookup<ProjectileData>(true),

            ECB = ecb // 단일 ECB 전달
        };

        // Job 스케줄링
        state.Dependency = job.Schedule(simulation, state.Dependency);

        // Job 완료 후 ECB 실행
        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
