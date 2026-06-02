using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(AuraSystem))]
[UpdateAfter(typeof(ProjectileSpawnSystem))]
[UpdateAfter(typeof(SpatialPartitionBuildSystem))]
public partial struct ChainLightningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ChainLightningData>();
        state.RequireForUpdate<ChainLightningBaseStatsData>();
        state.RequireForUpdate<ChainLightningStatsData>();
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
        NativeList<Entity> targetCandidates = new NativeList<Entity>(Allocator.Temp);
        NativeList<Entity> chainedTargets = new NativeList<Entity>(Allocator.Temp);

        foreach (var (transformRO, chainStateRW, combatStatsRO, baseStatsRO, bonusStatsRO, sourceEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<ChainLightningData>,
                RefRO<CombatStatsData>,
                RefRO<ChainLightningBaseStatsData>,
                RefRO<ChainLightningStatsData>>()
            .WithEntityAccess())
        {
            ref ChainLightningData chainState = ref chainStateRW.ValueRW;
            ref readonly CombatStatsData combatStats = ref combatStatsRO.ValueRO;
            ref readonly ChainLightningBaseStatsData baseStats = ref baseStatsRO.ValueRO;
            ref readonly ChainLightningStatsData bonusStats = ref bonusStatsRO.ValueRO;

            // 기본값, 개별 보정, 공통 전투 배율을 조합해 최종 공격 속도를 계산한다.
            float finalAttackSpeed =
                baseStats.BaseAttackSpeed *
                (1f + bonusStats.AttackSpeedBonusRate) *
                combatStats.AttackSpeed;

            chainState.ElapsedTime += deltaTime * finalAttackSpeed;

            if (chainState.ElapsedTime < 1f)
                continue;

            int castCount = (int)chainState.ElapsedTime;
            chainState.ElapsedTime %= 1f;

            float3 sourcePosition = transformRO.ValueRO.Position;

            for (int castIndex = 0; castIndex < castCount; castIndex++)
            {
                ExecuteChain(
                    sourceEntity,
                    sourcePosition,
                    chainState.OwnerFaction,
                    baseStats,
                    bonusStats,
                    combatStats,
                    spatialIndex,
                    transformLookup,
                    factionLookup,
                    healthLookup,
                    deadLookup,
                    ref targetCandidates,
                    ref chainedTargets,
                    ref ecb);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        targetCandidates.Dispose();
        chainedTargets.Dispose();
    }

    private static void ExecuteChain(
        Entity sourceEntity,
        float3 sourcePosition,
        Faction ownerFaction,
        in ChainLightningBaseStatsData baseStats,
        in ChainLightningStatsData bonusStats,
        in CombatStatsData combatStats,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        ref NativeList<Entity> targetCandidates,
        ref NativeList<Entity> chainedTargets,
        ref EntityCommandBuffer ecb)
    {
        // 첫 타깃 탐색과 이후 점프 탐색은 서로 다른 반경 보정값을 사용한다.
        float firstTargetSearchRadius = (baseStats.BaseAcquireRadius + bonusStats.AcquireRadiusBonus) * combatStats.AttackRange;
        float nextTargetSearchRadius = (baseStats.BaseJumpRadius + bonusStats.JumpRadiusBonus) * combatStats.AttackRange;
        int maxChainTargets = math.max(1, baseStats.BaseMaxTargets + bonusStats.MaxTargetsBonus);
        float damageMultiplierPerJump = math.max(0f, baseStats.BaseDamageMultiplierPerJump + bonusStats.DamageMultiplierPerJumpBonus);
        targetCandidates.Clear();
        chainedTargets.Clear();
        FixedList512Bytes<ChainLightningSegment> visualSegments = default;

        Entity currentSourceEntity = sourceEntity;
        float3 currentSourcePosition = sourcePosition;
        bool firstHop = true;

        for (int hopIndex = 0; hopIndex < maxChainTargets; hopIndex++)
        {
            float searchRadius = firstHop ? firstTargetSearchRadius : nextTargetSearchRadius;

            Entity targetEntity = FindNearestTarget(
                currentSourceEntity,
                currentSourcePosition,
                searchRadius,
                ownerFaction,
                chainedTargets,
                spatialIndex,
                transformLookup,
                factionLookup,
                healthLookup,
                deadLookup,
                ref targetCandidates);

            if (targetEntity == Entity.Null)
                break;

            float hopDamageMultiplier = math.pow(damageMultiplierPerJump, hopIndex);
            float finalDamage =
                baseStats.BaseDamage *
                (1f + bonusStats.DamageBonusRate) *
                combatStats.Damage *
                hopDamageMultiplier;

            // 전투 결과는 DamageEvent로 발행해 데미지 적용 시스템이 공통 처리한다.
            Entity damageEventEntity = ecb.CreateEntity();
            ecb.AddComponent(damageEventEntity, new DamageEventData
            {
                Target = targetEntity,
                Damage = finalDamage
            });

            float3 targetPosition = transformLookup[targetEntity].Position;

            visualSegments.Add(new ChainLightningSegment
            {
                From = currentSourcePosition,
                To = targetPosition
            });

            chainedTargets.Add(targetEntity);
            currentSourceEntity = targetEntity;
            currentSourcePosition = targetPosition;
            firstHop = false;
        }

        if (visualSegments.Length > 0)
        {
            // 연출은 VisualEvent로 분리해 Presentation 계층에서 소비한다.
            Entity visualEventEntity = ecb.CreateEntity();
            ecb.AddComponent(visualEventEntity, new ChainLightningVisualEvent
            {
                Segments = visualSegments,
                Duration = 0.12f
            });
        }
    }

    private static Entity FindNearestTarget(
        Entity excludedEntity,
        float3 center,
        float radius,
        Faction ownerFaction,
        in NativeList<Entity> alreadyHitTargets,
        in SpatialIndex spatialIndex,
        in ComponentLookup<LocalTransform> transformLookup,
        in ComponentLookup<FactionData> factionLookup,
        in ComponentLookup<HealthData> healthLookup,
        in ComponentLookup<DeadTag> deadLookup,
        ref NativeList<Entity> candidates)
    {
        SpatialUtility.QueryRadius(center, radius, spatialIndex.CellSize, spatialIndex.Map, ref candidates);

        float radiusSq = radius * radius;
        float nearestDistanceSq = float.MaxValue;
        Entity nearestTarget = Entity.Null;

        for (int i = 0; i < candidates.Length; i++)
        {
            Entity candidate = candidates[i];

            if (candidate == excludedEntity)
                continue;

            if (!transformLookup.HasComponent(candidate) ||
                !factionLookup.HasComponent(candidate) ||
                !healthLookup.HasComponent(candidate))
                continue;

            if (deadLookup.HasComponent(candidate))
                continue;

            if (factionLookup[candidate].Value == ownerFaction)
                continue;

            if (ContainsEntity(alreadyHitTargets, candidate))
                continue;

            float3 delta = transformLookup[candidate].Position - center;
            float distanceSq = math.lengthsq(delta);

            if (distanceSq > radiusSq)
                continue;

            if (distanceSq >= nearestDistanceSq)
                continue;

            nearestDistanceSq = distanceSq;
            nearestTarget = candidate;
        }

        return nearestTarget;
    }

    private static bool ContainsEntity(in NativeList<Entity> entities, Entity candidate)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i] == candidate)
                return true;
        }

        return false;
    }
}
