using UnityEngine;

[CreateAssetMenu(fileName = "UnlockChainLightningConfig", menuName = "Config/Base/Unlock/ChainLightningConfig")]
public class UnlockChainLightningConfig : UnlockAbilityConfig
{
    [SerializeField] private float baseDamage = 15f;
    [SerializeField] private float baseAttackSpeed = 1f;
    [SerializeField] private float baseAcquireRadius = 8f;
    [SerializeField] private float baseJumpRadius = 5f;
    [SerializeField] private int baseMaxTargets = 3;
    [SerializeField] private float baseDamageMultiplierPerJump = 0.85f;

    public override async void ApplyToPlayer()
    {
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, ChainLightningData>(new ChainLightningData
        {
            ElapsedTime = 0f,
            OwnerFaction = Faction.Player
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, ChainLightningBaseStatsData>(new ChainLightningBaseStatsData
        {
            BaseDamage = baseDamage,
            BaseAttackSpeed = baseAttackSpeed,
            BaseAcquireRadius = baseAcquireRadius,
            BaseJumpRadius = baseJumpRadius,
            BaseMaxTargets = baseMaxTargets,
            BaseDamageMultiplierPerJump = baseDamageMultiplierPerJump
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, ChainLightningStatsData>(new ChainLightningStatsData());
    }
}
