using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(BlackHoleCastSystem))]
[UpdateAfter(typeof(ProjectileSpawnSystem))]
public partial struct BoomerangCastSystem : ISystem
{
    private const float MaxSpreadAngleRadians = 0.7853982f;
    private const float DefaultSpinSpeedRadians = 14f;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoomerangData>();
        state.RequireForUpdate<BoomerangBaseStatsData>();
        state.RequireForUpdate<BoomerangStatsData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;
        ComponentLookup<ShooterData> shooterLookup = SystemAPI.GetComponentLookup<ShooterData>(true);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transformRO, boomerangRW, combatStatsRO, boomerangBaseStatsRO, boomerangStatsRO, sourceEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<BoomerangData>,
                RefRO<CombatStatsData>,
                RefRO<BoomerangBaseStatsData>,
                RefRO<BoomerangStatsData>>()
            .WithEntityAccess())
        {
            ref BoomerangData boomerangData = ref boomerangRW.ValueRW;
            ref readonly CombatStatsData combatStats = ref combatStatsRO.ValueRO;
            ref readonly BoomerangBaseStatsData boomerangBaseStats = ref boomerangBaseStatsRO.ValueRO;
            ref readonly BoomerangStatsData boomerangStats = ref boomerangStatsRO.ValueRO;

            float finalAttackSpeed =
                boomerangBaseStats.BaseAttackSpeed *
                (1f + boomerangStats.AttackSpeedBonusRate) *
                combatStats.AttackSpeed;

            boomerangData.ElapsedTime += deltaTime * finalAttackSpeed;

            if (boomerangData.ElapsedTime < 1f)
                continue;

            int castCount = (int)boomerangData.ElapsedTime;
            boomerangData.ElapsedTime %= 1f;

            float3 sourcePosition = transformRO.ValueRO.Position;
            float3 baseDirection = ResolveBaseDirection(sourceEntity, transformRO.ValueRO.Rotation, shooterLookup);

            for (int castIndex = 0; castIndex < castCount; castIndex++)
            {
                SpawnBoomerangs(
                    sourceEntity,
                    sourcePosition,
                    baseDirection,
                    boomerangData,
                    boomerangBaseStats,
                    boomerangStats,
                    combatStats,
                    ref ecb);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private static void SpawnBoomerangs(
        Entity sourceEntity,
        float3 sourcePosition,
        float3 baseDirection,
        in BoomerangData boomerangData,
        in BoomerangBaseStatsData boomerangBaseStats,
        in BoomerangStatsData boomerangStats,
        in CombatStatsData combatStats,
        ref EntityCommandBuffer ecb)
    {
        int count = math.max(1, boomerangBaseStats.BaseProjectileCount + boomerangStats.ProjectileCountBonus);
        float totalAngle = MaxSpreadAngleRadians * (1f - 1f / count);
        float angleStep = count > 1 ? totalAngle / (count - 1) : 0f;
        float startAngle = -totalAngle * 0.5f;

        float finalDamage =
            boomerangBaseStats.BaseDamage *
            (1f + boomerangStats.DamageBonusRate) *
            combatStats.Damage;

        float finalSpeed =
            boomerangBaseStats.BaseSpeed *
            (1f + boomerangStats.SpeedBonusRate);

        float finalReturnSpeed =
            boomerangBaseStats.BaseReturnSpeed *
            (1f + boomerangStats.ReturnSpeedBonusRate);

        float finalReturnAcceleration =
            boomerangBaseStats.BaseReturnAcceleration *
            (1f + boomerangStats.ReturnSpeedBonusRate);

        float finalMaxDistance =
            math.max(0.5f, (boomerangBaseStats.BaseMaxDistance + boomerangStats.MaxDistanceBonus) * combatStats.AttackRange);

        float finalHitRadius =
            math.max(0.1f, (boomerangBaseStats.BaseHitRadius + boomerangStats.HitRadiusBonus) * combatStats.AttackRange);

        float finalHitCooldown =
            math.max(0.05f, boomerangBaseStats.BaseHitCooldown + boomerangStats.HitCooldownBonus);

        for (int i = 0; i < count; i++)
        {
            Entity boomerangProjectile = ecb.Instantiate(boomerangData.ProjectilePrefab);
            ecb.RemoveComponent<ProjectileData>(boomerangProjectile);

            float angle = startAngle + angleStep * i;
            float3 castDirection = math.mul(quaternion.RotateY(angle), baseDirection);
            float3 spawnPosition =
                sourcePosition +
                boomerangData.MuzzleOffset +
                castDirection * boomerangData.MuzzleDistance;

            ecb.SetComponent(boomerangProjectile, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.LookRotationSafe(castDirection, math.up()),
                Scale = 1f
            });

            ecb.AddComponent(boomerangProjectile, new BoomerangProjectileData
            {
                OwnerEntity = sourceEntity,
                OwnerFaction = boomerangData.OwnerFaction,
                Direction = castDirection,
                Speed = finalSpeed,
                ReturnSpeed = finalReturnSpeed,
                ReturnAcceleration = finalReturnAcceleration,
                CurrentReturnSpeed = finalSpeed,
                Damage = finalDamage,
                MaxDistance = finalMaxDistance,
                HitRadius = finalHitRadius,
                HitCooldown = finalHitCooldown,
                DistanceTraveled = 0f,
                SpinAngle = 0f,
                SpinSpeed = (i & 1) == 0 ? DefaultSpinSpeedRadians : -DefaultSpinSpeedRadians,
                IsReturning = false
            });
            ecb.AddBuffer<BoomerangRecentHitData>(boomerangProjectile);
        }
    }

    private static float3 ResolveBaseDirection(
        Entity sourceEntity,
        quaternion sourceRotation,
        in ComponentLookup<ShooterData> shooterLookup)
    {
        if (shooterLookup.HasComponent(sourceEntity))
        {
            float3 shooterDirection = shooterLookup[sourceEntity].Direction;
            if (math.lengthsq(shooterDirection) > 0.0001f)
                return math.normalize(shooterDirection);
        }

        return math.normalize(math.mul(sourceRotation, new float3(0f, 0f, 1f)));
    }
}
