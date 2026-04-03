using UnityEngine;

[CreateAssetMenu(fileName = "UnlockMeteorStrikeConfig", menuName = "Config/Base/Unlock/MeteorStrikeConfig")]
public class UnlockMeteorStrikeConfig : UnlockAbilityConfig
{
    [SerializeField] private float baseDamage = 45f;
    [SerializeField] private float baseAttackSpeed = 0.4f;
    [SerializeField] private float baseAcquireRadius = 10f;
    [SerializeField] private float baseImpactRadius = 2.5f;
    [SerializeField] private float baseImpactDelay = 0.8f;
    [SerializeField] private int baseMeteorCount = 1;
    [SerializeField] private float baseScatterRadius = 1.5f;

    public override async void ApplyToPlayer()
    {
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, MeteorStrikeData>(new MeteorStrikeData
        {
            ElapsedTime = 0f,
            OwnerFaction = Faction.Player
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, MeteorStrikeBaseStatsData>(new MeteorStrikeBaseStatsData
        {
            BaseDamage = baseDamage,
            BaseAttackSpeed = baseAttackSpeed,
            BaseAcquireRadius = baseAcquireRadius,
            BaseImpactRadius = baseImpactRadius,
            BaseImpactDelay = baseImpactDelay,
            BaseMeteorCount = baseMeteorCount,
            BaseScatterRadius = baseScatterRadius
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, MeteorStrikeStatsData>(new MeteorStrikeStatsData());
    }
}
