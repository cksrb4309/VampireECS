using Unity.Entities;

public struct EnemySpawnerData : IComponentData
{
    public int NextIndex;

    public float Radius;

    public float BaseInterval;
    public float SpawnInterval;

    public float Timer;
    public float ElapsedTime;

    public int BatchCount;

    public bool IsEnabled;
}