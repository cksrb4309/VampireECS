using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

public class ShooterAuthoring : MonoBehaviour
{
    public Faction OwnerFaction;

    public GameObject ProjectilePrefab;

    public Vector3 MuzzleOffset;

    public float MuzzleDistance;

    [FoldoutGroup("Shooter Stats")]
    public float Damage = 10f;

    [FoldoutGroup("Shooter Stats")]
    public float AttackSpeed = 1f;

    [FoldoutGroup("Shooter Stats")]
    public float Speed = 1f;

    [FoldoutGroup("Shooter Stats")]
    public int Count = 1;

    [FoldoutGroup("Shooter Stats")]
    public float Duration = 1f;

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
            AddComponent(entity, new ShooterStatsData
            {
                Damage = authoring.Damage,
                AttackSpeed = authoring.AttackSpeed,
                ProjectileSpeed = authoring.Speed,
                ProjectileCount = authoring.Count,
                ProjectileDuration = authoring.Duration
            });
            AddComponent(entity, new ShooterCanFireData
            {
                CanFire = authoring.StartCanFire
            });
        }
    }
}