using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct RenderTrailData : IComponentData
{
    public Entity Prefab;
    public float SpawnDistance;
    public float LifeTime;

    // 마지막 잔상 생성 위치
    public float3 LastSpawnPos;

    public FixedList32Bytes<float> KeyTimes;   
    public FixedList32Bytes<float> KeyScales;
}