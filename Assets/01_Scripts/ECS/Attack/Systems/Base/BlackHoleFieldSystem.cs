using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(EnemyMoveSystem))]
[UpdateAfter(typeof(BlackHoleCastSystem))]
[UpdateAfter(typeof(SpatialPartitionBuildSystem))]
public partial struct BlackHoleFieldSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BlackHoleFieldData>();
        state.RequireForUpdate<SpatialIndex>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;

        state.Dependency.Complete();

        SpatialIndex spatialIndex = SystemAPI.GetSingleton<SpatialIndex>();
        ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        ComponentLookup<FactionData> factionLookup = SystemAPI.GetComponentLookup<FactionData>(true);
        ComponentLookup<HealthData> healthLookup = SystemAPI.GetComponentLookup<HealthData>(true);
        ComponentLookup<DeadTag> deadLookup = SystemAPI.GetComponentLookup<DeadTag>(true);
        ComponentLookup<PhysicsVelocity> velocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(false);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        NativeList<Entity> candidates = new NativeList<Entity>(Allocator.Temp);

        foreach (var (fieldRW, fieldEntity) in
            SystemAPI.Query<RefRW<BlackHoleFieldData>>().WithEntityAccess())
        {
            ref BlackHoleFieldData field = ref fieldRW.ValueRW;

            field.RemainingDuration -= deltaTime;
            field.TickElapsedTime += deltaTime;

            int tickCount = 0;
            if (field.TickInterval > 0f)
            {
                tickCount = (int)math.floor(field.TickElapsedTime / field.TickInterval);
                if (tickCount > 0)
                {
                    field.TickElapsedTime -= field.TickInterval * tickCount;
                }
            }

            ApplyFieldEffects(
                field,
                tickCount,
                spatialIndex,
                transformLookup,
                factionLookup,
                healthLookup,
                deadLookup,
                velocityLookup,
                ref candidates,
                ref ecb);

            if (field.RemainingDuration <= 0f)
            {
                ecb.DestroyEntity(fieldEntity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        candidates.Dispose();
    }

    private static void ApplyFieldEffects(
        in BlackHoleFieldData field,
        int tickCount,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        ComponentLookup<PhysicsVelocity> velocityLookup,
        ref NativeList<Entity> candidates,
        ref EntityCommandBuffer ecb)
    {
        SpatialUtility.QueryRadius(field.Position, field.Radius, spatialIndex.CellSize, spatialIndex.Map, ref candidates);

        float radiusSq = field.Radius * field.Radius;

        for (int i = 0; i < candidates.Length; i++)
        {
            Entity candidate = candidates[i];

            if (!transformLookup.HasComponent(candidate) ||
                !factionLookup.HasComponent(candidate) ||
                !healthLookup.HasComponent(candidate))
                continue;

            if (deadLookup.HasComponent(candidate))
                continue;

            if (factionLookup[candidate].Value == field.OwnerFaction)
                continue;

            float3 delta = field.Position - transformLookup[candidate].Position;
            float distanceSq = math.lengthsq(delta);

            if (distanceSq > radiusSq)
                continue;

            float distance = math.sqrt(distanceSq);
            float3 pullDirection = math.normalizesafe(delta);

            if (velocityLookup.HasComponent(candidate))
            {
                PhysicsVelocity velocity = velocityLookup[candidate];
                float pullWeight = math.lerp(0.4f, 1f, math.saturate(1f - distance / math.max(field.Radius, 0.001f)));
                velocity.Linear += pullDirection * field.PullStrength * pullWeight;
                velocityLookup[candidate] = velocity;
            }

            if (tickCount <= 0)
                continue;

            Entity damageEventEntity = ecb.CreateEntity();
            ecb.AddComponent(damageEventEntity, new DamageEventData
            {
                Target = candidate,
                Damage = field.TickDamage * tickCount
            });
        }
    }
}
