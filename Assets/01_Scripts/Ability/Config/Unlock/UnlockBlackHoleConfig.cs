using UnityEngine;

[CreateAssetMenu(fileName = "UnlockBlackHoleConfig", menuName = "Config/Base/Unlock/BlackHoleConfig")]
public class UnlockBlackHoleConfig : UnlockAbilityConfig
{
    [SerializeField] private float baseDamage = 4f;
    [SerializeField] private float baseAttackSpeed = 0.35f;
    [SerializeField] private float baseAcquireRadius = 10f;
    [SerializeField] private float baseRadius = 4f;
    [SerializeField] private float baseDuration = 2.5f;
    [SerializeField] private float baseTickInterval = 0.25f;
    [SerializeField] private float basePullStrength = 7.5f;

    public override async void ApplyToPlayer()
    {
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, BlackHoleData>(new BlackHoleData
        {
            ElapsedTime = 0f,
            OwnerFaction = Faction.Player
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, BlackHoleBaseStatsData>(new BlackHoleBaseStatsData
        {
            BaseDamage = baseDamage,
            BaseAttackSpeed = baseAttackSpeed,
            BaseAcquireRadius = baseAcquireRadius,
            BaseRadius = baseRadius,
            BaseDuration = baseDuration,
            BaseTickInterval = baseTickInterval,
            BasePullStrength = basePullStrength
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, BlackHoleStatsData>(new BlackHoleStatsData());
    }
}
