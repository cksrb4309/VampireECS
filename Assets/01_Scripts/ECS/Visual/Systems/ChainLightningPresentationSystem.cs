using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(DestructionCleanupSystemGroup))]
public partial struct ChainLightningPresentationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (visualEventRO, visualEventEntity) in
                 SystemAPI.Query<RefRO<ChainLightningVisualEvent>>().WithEntityAccess())
        {
            ref readonly ChainLightningVisualEvent visualEvent = ref visualEventRO.ValueRO;

            // 실제 View 생성과 갱신은 Presentation 계층의 ViewManager에 위임한다.
            ChainLightningViewManager.Instance.Emit(visualEvent.Segments, visualEvent.Duration);

            ecb.DestroyEntity(visualEventEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
