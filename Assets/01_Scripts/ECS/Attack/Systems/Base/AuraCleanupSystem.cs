using Unity.Entities;

public partial struct AuraCleanupSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<AuraVFXID, DeadTag>()
            .Build());
    }
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (auraIdRO, entity)
            in SystemAPI.Query<AuraVFXID>().WithAll<DeadTag>().WithEntityAccess())
        {
            AuraViewManager.Instance.RemoveView(auraIdRO.Value);

            // 현재는 이후 시스템 그룹에서 엔티티를 바로 제거하기 때문에 필요없지만
            // 만약 엔티티를 바로 제거하지 않는다면 아래 내용을 고려해봐야함
            // ecb.RemoveComponent<AuraVFXID>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
