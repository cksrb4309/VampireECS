using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyTeleportSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }
    public void OnUpdate(ref SystemState state)
    {
        Entity player = SystemAPI.GetSingletonEntity<PlayerTag>();
        float3 playerPos = state.EntityManager.GetComponentData<LocalToWorld>(player).Position;

        EnemySpawnerData spawnerData = SystemAPI.GetSingleton<EnemySpawnerData>();
        float radius = spawnerData.Radius;

        var job = new EnemyTeleportJob
        {
            Distance = radius * 1.5f,
            Radius = radius,
            PlayerPos = playerPos,
            Random = new Random((uint)(SystemAPI.Time.ElapsedTime * 123523 % 1000000) + 1)
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}
