using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float Speed = 5f;
    public int ExperienceAmount = 1;
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
}
