using Unity.Entities;

[UpdateInGroup(typeof(DeathSystemGroup))]
public partial struct EnemyDeathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        
        foreach (var (experienceDataRO, entity) in SystemAPI.Query<RefRO<ExperienceData>>().WithEntityAccess().WithAll<EnemyTag, DeadTag>())
        {
            Entity experienceEvent = ecb.CreateEntity();

            ecb.AddComponent(experienceEvent, new ExperienceGainEvent { Amount = experienceDataRO.ValueRO.Amount });

            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
