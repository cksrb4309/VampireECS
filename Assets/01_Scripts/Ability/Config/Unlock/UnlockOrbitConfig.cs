using UnityEngine;

[CreateAssetMenu(fileName = "UnlockOrbitConfig", menuName = "Config/Base/Unlock/OrbitConfig")]
public class UnlockOrbitConfig : UnlockAbilityConfig
{
    [SerializeField] private float baseDamage = 12f;
    [SerializeField] private float baseAttackSpeed = 0.25f;
    [SerializeField] private float baseOrbitSpeed = 2f;
    [SerializeField] private float baseOrbitRadius = 3f;
    [SerializeField] private float baseHitRadius = 0.8f;
    [SerializeField] private int baseProjectileCount = 2;
    [SerializeField] private float baseHitCooldown = 0.5f;

    public override async void ApplyToPlayer()
    {
        AbilityPrefabLibrary prefabLibrary = await EntityUtility.GetOrWaitForSingletonComponentAsync<AbilityPrefabLibrary>();

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, OrbitData>(new OrbitData
        {
            ProjectilePrefab = prefabLibrary.OrbitProjectilePrefab,
            OwnerFaction = Faction.Player,
            ElapsedTime = 0f
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, OrbitBaseStatsData>(new OrbitBaseStatsData
        {
            BaseDamage = baseDamage,
            BaseAttackSpeed = baseAttackSpeed,
            BaseOrbitSpeed = baseOrbitSpeed,
            BaseOrbitRadius = baseOrbitRadius,
            BaseHitRadius = baseHitRadius,
            BaseProjectileCount = baseProjectileCount,
            BaseHitCooldown = baseHitCooldown
        });

        OrbitStatsData statsData = new OrbitStatsData();
        statsData.Initialize();
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, OrbitStatsData>(statsData);
    }
}
