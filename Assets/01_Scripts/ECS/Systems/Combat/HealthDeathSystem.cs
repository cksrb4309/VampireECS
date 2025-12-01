using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct HealthDeathSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HealthData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        foreach (var (health, entity) in SystemAPI.Query<RefRW<HealthData>>().WithEntityAccess())
        {
            if (health.ValueRW.Current <= 0f)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}
