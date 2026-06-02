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

        foreach (var (damageEventRO, damageEventEntity) in
                 SystemAPI.Query<RefRO<DamageEventData>>().WithEntityAccess())
        {
            ref readonly DamageEventData damageEvent = ref damageEventRO.ValueRO;
            Entity target = damageEvent.Target;

            // 이미 사라진 타깃의 이벤트는 소비만 하고 후속 이벤트를 만들지 않는다.
            if (!SystemAPI.Exists(target) || !SystemAPI.HasComponent<HealthData>(target))
            {
                ecb.DestroyEntity(damageEventEntity);
                continue;
            }

            // 데미지 표시는 별도 이벤트로 발행해 전투 처리와 연출을 분리한다.
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
                ecb.DestroyEntity(damageEventEntity);
                continue;
            }

            health.Current -= damageEvent.Damage;

            if (health.Current <= 0 && !SystemAPI.HasComponent<DeadTag>(target))
                ecb.AddComponent<DeadTag>(target);

            ecb.DestroyEntity(damageEventEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

