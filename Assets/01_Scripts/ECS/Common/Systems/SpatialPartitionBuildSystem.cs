using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


[UpdateInGroup(typeof(SpatialSetupSystemGroup))]
[UpdateAfter(typeof(SpatialPartitionUpdateSystem))]
public partial struct SpatialPartitionBuildSystem : ISystem
{
    private EntityQuery spatialCellQuery;

    public void OnCreate(ref SystemState state)
    {
        spatialCellQuery = state.GetEntityQuery(ComponentType.ReadOnly<SpatialCell>());
        state.RequireForUpdate(spatialCellQuery);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        var entity = ecb.CreateEntity();

        ecb.AddComponent(entity, new SpatialIndex
        {
            CellSize = 1f,
            Map = new NativeParallelMultiHashMap<int2, Entity>(1024, Allocator.Persistent)
        });

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    public void OnDestroy(ref SystemState state)
    {
        if (SystemAPI.TryGetSingletonRW<SpatialIndex>(out var index))
        {
            if (index.ValueRO.Map.IsCreated)
            {
                index.ValueRW.Map.Dispose();
            }
        }
    }
    public void OnUpdate(ref SystemState state)
    {
        var index = SystemAPI.GetSingletonRW<SpatialIndex>();
        int requiredCapacity = math.max(1024, spatialCellQuery.CalculateEntityCount());

        if (index.ValueRW.Map.Capacity < requiredCapacity)
        {
            index.ValueRW.Map.Capacity = math.max(requiredCapacity, index.ValueRW.Map.Capacity * 2);
        }

        index.ValueRW.Map.Clear();

        var writer = index.ValueRW.Map.AsParallelWriter();

        var job = new BuildIndexJob
        {
            Writer = writer
        };
        
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}
