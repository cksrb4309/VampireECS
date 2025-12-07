
using Unity.Entities;

public class ShooterBaker : Baker<ShooterAuthoring>
{
    public override void Bake(ShooterAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        var projectileEntity = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic);

        AddComponent(entity, new AttackDamageData { Damage = authoring.Damage });
        AddComponent(entity, new ProjectileDurationData { Duration = authoring.Duration });
        AddComponent(entity, new ProjectileSpeedData { Speed = authoring.Speed });
        AddComponent(entity, new ProjectileCountData { Count = authoring.Count });

        AddComponent(entity, new ShooterData
        {
            ProjectilePrefab = projectileEntity,
            MuzzleOffset = authoring.MuzzleOffset,
            MuzzleDistance = authoring.MuzzleDistance,
            FireRate = authoring.FireRate,
            TimeSinceLastFire = 0f,
            OwnerFaction = authoring.OwnerFaction
        });
        AddComponent(entity, new ShooterCanFireData
        {
            CanFire = authoring.StartCanFire
        });
    }
}