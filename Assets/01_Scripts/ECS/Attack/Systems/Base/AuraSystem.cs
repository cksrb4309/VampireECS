using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
public partial struct AuraSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AuraData>();
        state.RequireForUpdate<SpatialIndex>();
    }   
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;

        var spatialIndex = SystemAPI.GetSingleton<SpatialIndex>();

        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        var factionLookup = SystemAPI.GetComponentLookup<FactionData>(true);

        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var job = new AuraJob
        {
            CellSize = spatialIndex.CellSize,
            DeltaTime = deltaTime,

            CellMap = spatialIndex.Map,
            TransformLookup = transformLookup,
            FactionLookup = factionLookup,

            ECB = ecb
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}
