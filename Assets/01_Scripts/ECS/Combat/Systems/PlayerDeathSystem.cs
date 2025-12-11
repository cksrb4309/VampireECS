using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(DeathSystemGroup))]
public partial struct PlayerDeathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        foreach (var (health, entity) in SystemAPI.Query<RefRW<HealthData>>().WithEntityAccess().WithAll<PlayerTag>())
        {
            if (health.ValueRW.Current <= 0f)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}
