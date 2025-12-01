using Unity.Entities;

public class HealthBaker : Baker<HealthAuthoring>
{
    public override void Bake(HealthAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new HealthData { Current = authoring.Health, Max = authoring.Health });
    }
}
