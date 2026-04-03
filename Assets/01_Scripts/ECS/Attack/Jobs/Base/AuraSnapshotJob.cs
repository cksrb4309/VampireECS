using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct AuraSnapshotJob : IJobEntity
{
    public NativeList<AuraSnapshot>.ParallelWriter Output;

    void Execute(
        in AuraData aura,
        in AuraVFXID vfxID,
        in LocalTransform transform,
        in AuraBaseStatsData auraBaseStatsData,
        in AuraStatsData auraStatsData,
        in CombatStatsData combatStatsData)
    {
        Output.AddNoResize(new AuraSnapshot
        {
            Position = transform.Position,
            Radius =
                auraBaseStatsData.BaseRadius *
                (1f + auraStatsData.RadiusBonusRate) *
                combatStatsData.AttackRange,
            ViewID = vfxID.Value
        });
    }
}
