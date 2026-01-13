using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreDestructionCleanupSystemGroup))]
public partial struct DamageTextPresentationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var emitter = DamageTextParticleEmitter.Instance;

        if (emitter == null) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (evt, entity) in
                 SystemAPI.Query<RefRO<DamageTextEvent>>()
                          .WithEntityAccess())
        {
            var data = evt.ValueRO;

            emitter.SpawnParticle(
                data.WorldPosition,
                data.Damage.ToString(),
                new Color(data.Color.x, data.Color.y, data.Color.z, data.Color.w)
            );

            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
