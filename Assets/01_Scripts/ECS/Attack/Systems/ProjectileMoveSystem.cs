using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(DamageSetupSystemGroup))]
public partial struct ProjectileMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var job = new ProjectileMoveJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value,
            ECB = ecb
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}