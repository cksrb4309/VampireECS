using UnityEngine;

[CreateAssetMenu(fileName = "UnlockBlackHoleConfig", menuName = "Config/Base/Unlock/BlackHoleConfig")]
public class UnlockBlackHoleConfig : UnlockAbilityConfig
{
    public override async void ApplyToPlayer()
    {
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, BlackHoleData>(new BlackHoleData
        {
            ElapsedTime = 0f,
            OwnerFaction = Faction.Player
        });
    }
}
