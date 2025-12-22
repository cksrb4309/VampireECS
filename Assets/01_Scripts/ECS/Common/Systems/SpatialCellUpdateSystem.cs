using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SpatialCellSetupSystemGroup))]
[UpdateBefore(typeof(SpatialIndexBuildSystem))]
public partial struct SpatialCellUpdateSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpatialCell>();
        state.RequireForUpdate<SpatialIndex>();
    }
    public void OnUpdate(ref SystemState state)
    {
        float cellSize = SystemAPI.GetSingleton<SpatialIndex>().CellSize;

        foreach (var (transform, cell) in SystemAPI.Query<LocalTransform, RefRW<SpatialCell>>())
        {
            int2 newCell = (int2)math.floor(transform.Position.xz / cellSize);

            cell.ValueRW.Value = newCell;
        }
    }
}
