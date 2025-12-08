using Unity.Entities;

public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new ExperienceData { Amount = authoring.ExperienceAmount });

        AddComponent(entity, new EnemyMoveData { Speed = authoring.Speed });

        AddComponent<EnemyTag>(entity);
        AddComponent<PhysicsInit>(entity);
        AddComponent<LockYToZero>(entity);
    }
}