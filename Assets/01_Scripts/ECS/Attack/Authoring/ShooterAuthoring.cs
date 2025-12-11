using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

public class ShooterAuthoring : MonoBehaviour
{
    public Faction OwnerFaction;

    public GameObject ProjectilePrefab;

    public Vector3 MuzzleOffset;

    public float AttackSpeed = 1f;

    public float MuzzleDistance;

    [ShowIf("UseShooterStats")]
    [FoldoutGroup("Shooter Stats")]
    public float Damage = 10f;

    [ShowIf("UseShooterStats")]
    [FoldoutGroup("Shooter Stats")]
    public float Speed = 1f;

    [ShowIf("UseShooterStats")]
    [FoldoutGroup("Shooter Stats")]
    public int Count = 1;

    [ShowIf("UseShooterStats")]
    [FoldoutGroup("Shooter Stats")]
    public float Duration = 1f;

    public bool StartCanFire;
    public bool UseShooterStats = true;
    public class ShooterBaker : Baker<ShooterAuthoring>
    {
        public override void Bake(ShooterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var projectileEntity = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic);

            if (authoring.UseShooterStats)
            {
                AddComponent(entity, new AttackDamageData { Damage = authoring.Damage });
                AddComponent(entity, new ProjectileDurationData { Duration = authoring.Duration });
                AddComponent(entity, new ProjectileSpeedData { Speed = authoring.Speed });
                AddComponent(entity, new ProjectileCountData { Count = authoring.Count });
                AddComponent(entity, new AttackSpeedData { AttackSpeed = authoring.AttackSpeed });
            }

            AddComponent(entity, new ShooterData
            {
                ProjectilePrefab = projectileEntity,
                MuzzleOffset = authoring.MuzzleOffset,
                MuzzleDistance = authoring.MuzzleDistance,
                ElapsedTime = 0f,
                OwnerFaction = authoring.OwnerFaction
            });
            AddComponent(entity, new ShooterCanFireData
            {
                CanFire = authoring.StartCanFire
            });
        }
    }
}