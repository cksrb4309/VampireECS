using System;
using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public EnemySpawnStage[] Stages;

    public float SpawnInterval = 2f;
    public float Radius = 30f;

    public int BatchCount = 1;
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
}

[Serializable]
public struct EnemySpawnStage
{
    public GameObject Prefab;   // 스폰할 몬스터
    public float StartTime;     // 이 시간부터 등장
}
public struct EnemySpawnElement : IBufferElementData
{
    public Entity EnemyPrefab;
    public float StartTime;
}