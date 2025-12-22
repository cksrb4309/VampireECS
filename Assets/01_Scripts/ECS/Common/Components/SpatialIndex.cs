using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct SpatialIndex :IComponentData
{
    public float CellSize;

    public NativeParallelMultiHashMap<int2, Entity> Map;
}
