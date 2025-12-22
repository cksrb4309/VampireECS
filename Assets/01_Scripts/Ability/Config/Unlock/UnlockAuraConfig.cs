using UnityEngine;

[CreateAssetMenu(fileName = "UnlockAuraConfig", menuName = "Config/Base/Unlock/AuraConfig")]
public class UnlockAuraConfig : UnlockAbilityConfig
{
    public override async void ApplyToPlayer()
    {
        int viewID = AuraViewManager.Instance.CreateView();

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, AuraVFXID>(new AuraVFXID
        {
            Value = viewID
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, AuraData>(new AuraData
        {
            ElapsedTime = 0,
            OwnerFaction = Faction.Player
        });
    }
}
