using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct RenderTrailSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DynamicRotationData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var job = new RenderTrailJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECB = ecb
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}