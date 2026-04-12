using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(DestructionCleanupSystemGroup))]
public partial struct ChainLightningPresentationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (eventRO, eventEntity) in
                 SystemAPI.Query<RefRO<ChainLightningVisualEvent>>().WithEntityAccess())
        {
            ref readonly ChainLightningVisualEvent visualEvent = ref eventRO.ValueRO;

            ChainLightningViewManager.Instance.Emit(visualEvent.Segments, visualEvent.Duration);

            ecb.DestroyEntity(eventEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
