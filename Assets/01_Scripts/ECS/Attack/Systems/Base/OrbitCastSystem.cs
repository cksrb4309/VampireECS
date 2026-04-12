using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
[UpdateAfter(typeof(BoomerangCastSystem))]
public partial struct OrbitCastSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OrbitData>();
        state.RequireForUpdate<OrbitBaseStatsData>();
        state.RequireForUpdate<OrbitStatsData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transformRO, orbitRW, combatStatsRO, baseStatsRO, statsRO, sourceEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<OrbitData>,
                RefRO<CombatStatsData>,
                RefRO<OrbitBaseStatsData>,
                RefRO<OrbitStatsData>>()
            .WithEntityAccess())
        {
            ref OrbitData orbitData = ref orbitRW.ValueRW;
            ref readonly CombatStatsData combatStats = ref combatStatsRO.ValueRO;
            ref readonly OrbitBaseStatsData baseStats = ref baseStatsRO.ValueRO;
            ref readonly OrbitStatsData stats = ref statsRO.ValueRO;

            float finalAttackSpeed =
                baseStats.BaseAttackSpeed *
                (1f + stats.AttackSpeedBonusRate) *
                combatStats.AttackSpeed;

            orbitData.ElapsedTime += deltaTime * finalAttackSpeed;

            if (orbitData.ElapsedTime < 1f)
                continue;

            int castCount = (int)orbitData.ElapsedTime;
            orbitData.ElapsedTime %= 1f;

            float3 sourcePosition = transformRO.ValueRO.Position;

            for (int castIndex = 0; castIndex < castCount; castIndex++)
            {
                SpawnOrbiters(
                    sourceEntity,
                    sourcePosition,
                    orbitData,
                    baseStats,
                    stats,
                    combatStats,
                    finalAttackSpeed,
                    ref ecb);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private static void SpawnOrbiters(
        Entity sourceEntity,
        float3 sourcePosition,
        in OrbitData orbitData,
        in OrbitBaseStatsData baseStats,
        in OrbitStatsData stats,
        in CombatStatsData combatStats,
        float finalAttackSpeed,
        ref EntityCommandBuffer ecb)
    {
        int count = math.max(1, baseStats.BaseProjectileCount + stats.ProjectileCountBonus);
        float angleStep = (2f * math.PI) / count;

        float finalDamage =
            baseStats.BaseDamage *
            (1f + stats.DamageBonusRate) *
            combatStats.Damage;

        float finalOrbitSpeed =
            baseStats.BaseOrbitSpeed *
            (1f + stats.OrbitSpeedBonusRate);

        float finalOrbitRadius =
            math.max(0.5f, (baseStats.BaseOrbitRadius + stats.OrbitRadiusBonus) * combatStats.AttackRange);

        float finalHitRadius =
            math.max(0.1f, (baseStats.BaseHitRadius + stats.HitRadiusBonus) * combatStats.AttackRange);

        float finalHitCooldown =
            math.max(0.05f, baseStats.BaseHitCooldown + stats.HitCooldownBonus);

        // Lifetime slightly longer than spawn interval to prevent gaps
        float lifetime = (1f / math.max(0.01f, finalAttackSpeed)) * 1.1f;

        for (int i = 0; i < count; i++)
        {
            Entity orbitProjectile = ecb.Instantiate(orbitData.ProjectilePrefab);

            float startAngle = angleStep * i;
            float3 offset = new float3(math.cos(startAngle), 0f, math.sin(startAngle)) * finalOrbitRadius;
            float3 spawnPosition = sourcePosition + offset;

            ecb.SetComponent(orbitProjectile, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            ecb.AddComponent(orbitProjectile, new OrbitProjectileData
            {
                OwnerEntity = sourceEntity,
                OwnerFaction = orbitData.OwnerFaction,
                CurrentAngle = startAngle,
                OrbitSpeed = finalOrbitSpeed,
                OrbitRadius = finalOrbitRadius,
                Damage = finalDamage,
                HitRadius = finalHitRadius,
                HitCooldown = finalHitCooldown,
                RemainingLifetime = lifetime
            });

            ecb.AddBuffer<OrbitRecentHitData>(orbitProjectile);
        }
    }
}
