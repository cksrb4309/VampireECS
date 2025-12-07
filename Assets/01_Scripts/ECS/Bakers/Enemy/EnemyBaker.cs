using Unity.Entities;

public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent<PhysicsInit>(entity);

        AddComponent(entity, new ExperienceData { Amount = authoring.ExperienceAmount });

        AddComponent(entity, new EnemyMoveData
        {
            Speed = authoring.Speed
        });

        AddComponent<LockYToZero>(entity);
    }
}