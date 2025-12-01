using System;
using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public EnemySpawnStage[] Stages;

    public float SpawnInterval = 2f;
    public float Radius = 30f;

    public int BatchCount = 1;
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