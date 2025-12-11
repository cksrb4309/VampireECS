using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct LockYToZeroSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var transform
            in SystemAPI.Query<RefRW<LocalTransform>>()
                        .WithAll<LockYToZero>())
        {
            float3 pos = transform.ValueRO.Position;
            pos.y = 0f;
            transform.ValueRW.Position = pos;
        }
    }
}