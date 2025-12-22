using Unity.Entities;
using Unity.Mathematics;
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

            AddComponent(entity, new SpatialCell { Value = new int2 { x = 1000, y = 1000 } });
            AddComponent(entity, new FactionData { Value = Faction.Enemy });
            AddComponent(entity, new ExperienceData { Amount = authoring.ExperienceAmount });
            AddComponent(entity, new EnemyMoveData { Speed = authoring.Speed });

            AddComponent<EnemyTag>(entity);
            AddComponent<PhysicsInit>(entity);
            AddComponent<LockYToZero>(entity);
        }
    }
}
