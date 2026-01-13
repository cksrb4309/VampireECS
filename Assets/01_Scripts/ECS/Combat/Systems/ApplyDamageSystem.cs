using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(DamageApplySystemGroup))]
public partial struct ApplyDamageSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // HealthData와 DamageData Buffer가 있어야 실행
        state.RequireForUpdate<HealthData>();
    }
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (damageEventDataRO, entity) in
                 SystemAPI.Query<RefRO<DamageEventData>>().WithEntityAccess())
        {
            ref readonly DamageEventData damageEvent = ref damageEventDataRO.ValueRO;

            ref HealthData health = ref SystemAPI.GetComponentRW<HealthData>(damageEvent.Target).ValueRW;

            if (health.Current > 0)
            {
                health.Current -= damageEvent.Damage;


                { // DamageTextEvent 생성
                    float3 pos = SystemAPI.GetComponent<LocalTransform>(damageEvent.Target).Position;

                    Entity evt = ecb.CreateEntity();

                    ecb.AddComponent(evt, new DamageTextEvent
                    {
                        WorldPosition = pos,
                        Damage = (int)damageEvent.Damage,
                        Color = new float4(1, 0, 0, 1)
                    });
                }
                

                if (health.Current <= 0)
                {
                    ecb.AddComponent<DeadTag>(damageEvent.Target);
                }
            }

            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
