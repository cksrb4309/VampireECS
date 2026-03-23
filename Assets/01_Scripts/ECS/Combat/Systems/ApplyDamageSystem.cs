using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(DamageApplySystemGroup))]
public partial struct ApplyDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (damageEventDataRO, eventEntity) in
                 SystemAPI.Query<RefRO<DamageEventData>>().WithEntityAccess())
        {
            ref readonly DamageEventData damageEvent = ref damageEventDataRO.ValueRO;
            Entity target = damageEvent.Target;

            if (!SystemAPI.Exists(target) || !SystemAPI.HasComponent<HealthData>(target))
            {
                ecb.DestroyEntity(eventEntity);
                continue;
            }

            Entity damageTextEventEntity = ecb.CreateEntity();
            ecb.AddComponent(damageTextEventEntity, new DamageTextEvent
            {
                WorldPosition = SystemAPI.HasComponent<LocalTransform>(target)
                    ? SystemAPI.GetComponent<LocalTransform>(target).Position
                    : float3.zero,
                Damage = (int)damageEvent.Damage,
                Color = new float4(1, 0, 0, 1)
            });

            ref HealthData health = ref SystemAPI.GetComponentRW<HealthData>(target).ValueRW;

            if (health.Current <= 0)
            {
                ecb.DestroyEntity(eventEntity);
                continue;
            }

            health.Current -= damageEvent.Damage;

            if (health.Current <= 0 && !SystemAPI.HasComponent<DeadTag>(target))
                ecb.AddComponent<DeadTag>(target);

            ecb.DestroyEntity(eventEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

