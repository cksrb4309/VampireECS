using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct AuraJob : IJobEntity
{
    public float CellSize;
    public float DeltaTime;

    [ReadOnly] public NativeParallelMultiHashMap<int2, Entity> CellMap;

    [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
    [ReadOnly] public ComponentLookup<FactionData> FactionLookup;

    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute(
        [EntityIndexInQuery] int index,
        Entity auraEntity,
        in LocalTransform transform,
        ref AuraData auraData,
        in CombatStatsData playerStatsData,
        in AuraBaseStatsData auraBaseStatsData,
        in AuraStatsData auraStatsData)
    {
        float finalAttackSpeed =
            auraBaseStatsData.BaseAttackSpeed *
            (1f + auraStatsData.AttackSpeedBonusRate) *
            playerStatsData.AttackSpeed;

        auraData.ElapsedTime += DeltaTime * finalAttackSpeed;

        if (auraData.ElapsedTime < 1f) return;

        float3 center = transform.Position;
        int2 centerCell = SpatialUtility.WorldToCell(center, CellSize);

        float applyDamage =
            auraBaseStatsData.BaseDamage *
            (1f + auraStatsData.DamageBonusRate) *
            playerStatsData.Damage;

        float radius =
            auraBaseStatsData.BaseRadius *
            (1f + auraStatsData.RadiusBonusRate) *
            playerStatsData.AttackRange;

        float radiusSq = radius * radius;

        int cellRange = (int)math.ceil(radius / CellSize);
        int attackCount = (int)auraData.ElapsedTime;

        auraData.ElapsedTime %= 1f;

        for (int dx = -cellRange; dx <= cellRange; dx++)
        {
            for (int dz = -cellRange; dz <= cellRange; dz++)
            {
                int2 cell = centerCell + new int2(dx, dz);

                if (!CellMap.TryGetFirstValue(cell, out var targetEntity, out var it)) continue;

                do
                {
                    if (targetEntity == auraEntity) continue;

                    var targetTransform = TransformLookup[targetEntity];
                    var targetFaction = FactionLookup[targetEntity];

                    if (targetFaction.Value == auraData.OwnerFaction) continue;

                    float3 delta = targetTransform.Position - center;

                    if (math.lengthsq(delta) > radiusSq) continue;

                    var damageEvent = ECB.CreateEntity(index);
                    ECB.AddComponent(index, damageEvent, new DamageEventData
                    {
                        Target = targetEntity,
                        Damage = applyDamage * attackCount
                    });
                } while (CellMap.TryGetNextValue(out targetEntity, ref it));
            }
        }
    }
}
