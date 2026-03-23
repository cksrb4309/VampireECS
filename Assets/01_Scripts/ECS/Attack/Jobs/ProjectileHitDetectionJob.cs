using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public struct ProjectileHitDetectionJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<HealthData> HasHealthLookup;
    [ReadOnly] public ComponentLookup<ProjectileData> ProjectileLookup;

    public EntityCommandBuffer ECB; // 단일 ECB로 변경

    public void Execute(TriggerEvent triggerEvent)
    {
        if (EntityPairUtility.TryMatchPair(
            triggerEvent.EntityA,
            triggerEvent.EntityB,
            HasHealthLookup,
            ProjectileLookup,
            out Entity hit,
            out Entity projectile))
        {
            Entity damageEventEntity = ECB.CreateEntity();

            ECB.AddComponent(damageEventEntity, new DamageEventData { Target = hit, Damage = ProjectileLookup[projectile].Damage });

            // 단일 ECB이므로 index 없이 바로 Destroy
            ECB.DestroyEntity(projectile);
        }
    }
}
