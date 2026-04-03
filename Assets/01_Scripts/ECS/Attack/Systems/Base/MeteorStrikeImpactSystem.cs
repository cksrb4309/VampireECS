using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(MeteorStrikeCastSystem))]
[UpdateAfter(typeof(SpatialPartitionBuildSystem))]
public partial struct MeteorStrikeImpactSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MeteorStrikePendingData>();
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

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        NativeList<Entity> candidates = new NativeList<Entity>(Allocator.Temp);

        foreach (var (pendingRW, pendingEntity) in
            SystemAPI.Query<RefRW<MeteorStrikePendingData>>().WithEntityAccess())
        {
            ref MeteorStrikePendingData pending = ref pendingRW.ValueRW;

            pending.RemainingDelay -= deltaTime;

            if (pending.RemainingDelay > 0f)
                continue;

            ApplyMeteorImpact(
                pending,
                spatialIndex,
                transformLookup,
                factionLookup,
                healthLookup,
                deadLookup,
                ref candidates,
                ref ecb);

            Entity impactEventEntity = ecb.CreateEntity();
            ecb.AddComponent(impactEventEntity, new MeteorStrikeImpactVisualEvent
            {
                Position = pending.Position,
                Radius = pending.Radius,
                Duration = 0.28f,
                StrikeHeight = math.max(2.5f, pending.Radius * 2.5f)
            });

            ecb.DestroyEntity(pendingEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        candidates.Dispose();
    }

    private static void ApplyMeteorImpact(
        in MeteorStrikePendingData pending,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        ref NativeList<Entity> candidates,
        ref EntityCommandBuffer ecb)
    {
        SpatialUtility.QueryRadius(pending.Position, pending.Radius, spatialIndex.CellSize, spatialIndex.Map, ref candidates);

        float radiusSq = pending.Radius * pending.Radius;

        for (int i = 0; i < candidates.Length; i++)
        {
            Entity candidate = candidates[i];

            if (!transformLookup.HasComponent(candidate) ||
                !factionLookup.HasComponent(candidate) ||
                !healthLookup.HasComponent(candidate))
                continue;

            if (deadLookup.HasComponent(candidate))
                continue;

            if (factionLookup[candidate].Value == pending.OwnerFaction)
                continue;

            float3 delta = transformLookup[candidate].Position - pending.Position;

            if (math.lengthsq(delta) > radiusSq)
                continue;

            Entity damageEventEntity = ecb.CreateEntity();
            ecb.AddComponent(damageEventEntity, new DamageEventData
            {
                Target = candidate,
                Damage = pending.Damage
            });
        }
    }
}
