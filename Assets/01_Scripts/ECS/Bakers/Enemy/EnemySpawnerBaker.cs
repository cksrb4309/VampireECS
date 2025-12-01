using Unity.Entities;

public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake(EnemySpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        DynamicBuffer<EnemySpawnElement> buffer = AddBuffer<EnemySpawnElement>(entity);

        for (int i = 0; i < authoring.Stages.Length; i++)
        {
            buffer.Add(new EnemySpawnElement
            {
                EnemyPrefab = GetEntity(authoring.Stages[i].Prefab, TransformUsageFlags.Dynamic),
                StartTime = authoring.Stages[i].StartTime
            });
        }
        AddComponent(entity, new EnemySpawnerData
        {
            Radius = authoring.Radius,

            BaseInterval = authoring.SpawnInterval,
            SpawnInterval = authoring.SpawnInterval,

            Timer = authoring.SpawnInterval,
            ElapsedTime = 0,

            BatchCount = authoring.BatchCount,

            IsEnabled = true
        });
    }
}