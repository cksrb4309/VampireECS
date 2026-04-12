using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class ShooterAuthoring : MonoBehaviour
{
    public Faction OwnerFaction;

    public GameObject ProjectilePrefab;

    public Vector3 MuzzleOffset;

    public float MuzzleDistance;

    [FoldoutGroup("Shooter Stats")]
    [FormerlySerializedAs("Damage")]
    public float BaseDamage = 20f;

    [FoldoutGroup("Shooter Stats")]
    [FormerlySerializedAs("AttackSpeed")]
    public float BaseAttackSpeed = 5f;

    [FoldoutGroup("Shooter Stats")]
    [FormerlySerializedAs("Speed")]
    public float BaseProjectileSpeed = 20f;

    [FoldoutGroup("Shooter Stats")]
    [FormerlySerializedAs("Count")]
    public int BaseProjectileCount = 1;

    [FoldoutGroup("Shooter Stats")]
    [FormerlySerializedAs("Duration")]
    public float BaseProjectileDuration = 1f;

    public bool StartCanFire;
    public class ShooterBaker : Baker<ShooterAuthoring>
    {
        public override void Bake(ShooterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var projectileEntity = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShooterData
            {
                ProjectilePrefab = projectileEntity,
                MuzzleOffset = authoring.MuzzleOffset,
                MuzzleDistance = authoring.MuzzleDistance,
                ElapsedTime = 0f,
                OwnerFaction = authoring.OwnerFaction
            });
            AddComponent(entity, new ShooterBaseStatsData
            {
                BaseDamage = authoring.BaseDamage,
                BaseAttackSpeed = authoring.BaseAttackSpeed,
                BaseProjectileSpeed = authoring.BaseProjectileSpeed,
                BaseProjectileCount = authoring.BaseProjectileCount,
                BaseProjectileDuration = authoring.BaseProjectileDuration
            });
            AddComponent(entity, new ShooterStatsData
            {
                DamageBonusRate = 0f,
                AttackSpeedBonusRate = 0f,
                ProjectileSpeedBonusRate = 0f,
                ProjectileCountBonus = 0,
                ProjectileDurationBonus = 0f
            });
            AddComponent(entity, new ShooterCanFireData
            {
                CanFire = authoring.StartCanFire
            });
        }
    }
}
