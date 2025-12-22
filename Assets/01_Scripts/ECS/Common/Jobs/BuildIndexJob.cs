using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial struct BuildIndexJob : IJobEntity
{
    public NativeParallelMultiHashMap<int2, Entity>.ParallelWriter Writer;

    void Execute(Entity entity, in SpatialCell cell)
    {
        Writer.Add(cell.Value, entity);
    }
}
