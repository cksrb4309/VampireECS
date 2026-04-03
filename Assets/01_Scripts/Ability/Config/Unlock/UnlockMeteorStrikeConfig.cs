using UnityEngine;

[CreateAssetMenu(fileName = "UnlockMeteorStrikeConfig", menuName = "Config/Base/Unlock/MeteorStrikeConfig")]
public class UnlockMeteorStrikeConfig : UnlockAbilityConfig
{
    public override async void ApplyToPlayer()
    {
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, MeteorStrikeData>(new MeteorStrikeData
        {
            ElapsedTime = 0f,
            OwnerFaction = Faction.Player
        });
    }
}
