using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(DestructionCleanupSystemGroup))]
public partial struct MeteorStrikePresentationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (eventRO, eventEntity) in
                 SystemAPI.Query<RefRO<MeteorStrikeTelegraphVisualEvent>>().WithEntityAccess())
        {
            ref readonly MeteorStrikeTelegraphVisualEvent visualEvent = ref eventRO.ValueRO;

            MeteorStrikeViewManager.Instance.EmitTelegraph(
                visualEvent.Position,
                visualEvent.Radius,
                visualEvent.Duration);

            ecb.DestroyEntity(eventEntity);
        }

        foreach (var (eventRO, eventEntity) in
                 SystemAPI.Query<RefRO<MeteorStrikeImpactVisualEvent>>().WithEntityAccess())
        {
            ref readonly MeteorStrikeImpactVisualEvent visualEvent = ref eventRO.ValueRO;

            MeteorStrikeViewManager.Instance.EmitImpact(
                visualEvent.Position,
                visualEvent.Radius,
                visualEvent.Duration,
                visualEvent.StrikeHeight);

            ecb.DestroyEntity(eventEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
