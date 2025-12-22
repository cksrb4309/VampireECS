using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public static class SpatialUtility
{
    public static int2 WorldToCell(float3 position, float cellSize)
    {
        return (int2)math.floor(position.xz / cellSize);
    }
    public static float3 CellToWorldCenter(int2 cell, float cellSize)
    {
        return new float3(
            (cell.x + 0.5f) * cellSize,
            0f,
            (cell.y + 0.5f) * cellSize
        );
    }
    public static void QueryRadius(float3 center, float radius, float cellSize,
    NativeParallelMultiHashMap<int2, Entity> cellMap,
    ref NativeList<Entity> results)
    {
        results.Clear();

        int2 centerCell = WorldToCell(center, cellSize);

        int cellRange = (int)math.ceil(radius / cellSize);

        float radiusSq = radius * radius;

        for (int dx = -cellRange; dx <= cellRange; dx++)
        {
            for (int dz = -cellRange; dz <= cellRange; dz++)
            {
                int2 cell = centerCell + new int2(dx, dz);

                if (!cellMap.TryGetFirstValue(cell, out var entity, out var it)) continue;

                do
                {
                    results.Add(entity);
                }
                while (cellMap.TryGetNextValue(out entity, ref it));
            }
        }
    }
}
