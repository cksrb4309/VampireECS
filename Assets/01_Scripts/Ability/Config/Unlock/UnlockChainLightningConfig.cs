using UnityEngine;

[CreateAssetMenu(fileName = "UnlockChainLightningConfig", menuName = "Config/Base/Unlock/ChainLightningConfig")]
public class UnlockChainLightningConfig : UnlockAbilityConfig
{
    public override async void ApplyToPlayer()
    {
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, ChainLightningData>(new ChainLightningData
        {
            ElapsedTime = 0f,
            OwnerFaction = Faction.Player
        });
    }
}
