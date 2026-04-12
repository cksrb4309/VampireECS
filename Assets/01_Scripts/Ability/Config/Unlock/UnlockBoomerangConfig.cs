using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockBoomerangConfig", menuName = "Config/Base/Unlock/BoomerangConfig")]
public class UnlockBoomerangConfig : UnlockAbilityConfig
{
    [SerializeField] private float baseDamage = 15f;
    [SerializeField] private float baseAttackSpeed = 0.75f;
    [SerializeField] private float baseSpeed = 12f;
    [SerializeField] private float baseReturnSpeed = 67.5f;
    [SerializeField] private float baseReturnAcceleration = 20f;
    [SerializeField] private float baseMaxDistance = 11f;
    [SerializeField] private float baseHitRadius = 0.9f;
    [SerializeField] private int baseProjectileCount = 1;
    [SerializeField] private float baseHitCooldown = 0.2f;

    public override async void ApplyToPlayer()
    {
        AbilityPrefabLibrary prefabLibrary = await EntityUtility.GetOrWaitForSingletonComponentAsync<AbilityPrefabLibrary>();

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, BoomerangData>(new BoomerangData
        {
            ProjectilePrefab = prefabLibrary.BoomerangProjectilePrefab,
            MuzzleOffset = float3.zero,
            OwnerFaction = Faction.Player,
            MuzzleDistance = 1.5f,
            ElapsedTime = 0f
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, BoomerangBaseStatsData>(new BoomerangBaseStatsData
        {
            BaseDamage = baseDamage,
            BaseAttackSpeed = baseAttackSpeed,
            BaseSpeed = baseSpeed,
            BaseReturnSpeed = baseReturnSpeed,
            BaseReturnAcceleration = baseReturnAcceleration,
            BaseMaxDistance = baseMaxDistance,
            BaseHitRadius = baseHitRadius,
            BaseProjectileCount = baseProjectileCount,
            BaseHitCooldown = baseHitCooldown
        });

        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, BoomerangStatsData>(new BoomerangStatsData());
    }
}
