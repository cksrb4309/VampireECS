using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


[UpdateInGroup(typeof(SpatialSetupSystemGroup))]
[UpdateAfter(typeof(SpatialPartitionUpdateSystem))]
public partial struct SpatialPartitionBuildSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpatialCell>();

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

        index.ValueRW.Map.Clear();

        var writer = index.ValueRW.Map.AsParallelWriter();

        var job = new BuildIndexJob
        {
            Writer = writer
        };
        
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}
