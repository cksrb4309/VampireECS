using UnityEngine;

[CreateAssetMenu(fileName = "UnlockAuraConfig", menuName = "Config/Base/Unlock/AuraConfig")]
public class UnlockAuraConfig : UnlockAbilityConfig
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseAttackSpeed = 1f;
    [SerializeField] private float baseRadius = 10f;

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

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, AuraBaseStatsData>(new AuraBaseStatsData
        {
            BaseDamage = baseDamage,
            BaseAttackSpeed = baseAttackSpeed,
            BaseRadius = baseRadius
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, AuraStatsData>(new AuraStatsData());
    }
}
