using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(DestructionCleanupSystemGroup))]
public partial struct DamageTextSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (evt, entity) in
                 SystemAPI.Query<RefRO<DamageTextEvent>>()
                          .WithEntityAccess())
        {
            var data = evt.ValueRO;

            // DamageTextProvider를 통해 데미지 텍스트 출력 관제 (관심사 분리)
            DamageTextProvider.ShowDamageText(
                data.WorldPosition,
                data.Damage,
                new Color(data.Color.x, data.Color.y, data.Color.z, data.Color.w)
            );

            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
