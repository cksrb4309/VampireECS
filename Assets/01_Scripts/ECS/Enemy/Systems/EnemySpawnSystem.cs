using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawnerData>();
        state.RequireForUpdate<PlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime * SystemAPI.GetSingleton<GameTimeScale>().Value;
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (spawnerRW, spawnElements) in SystemAPI.Query<RefRW<EnemySpawnerData>, DynamicBuffer<EnemySpawnElement>>())
        {
            ref EnemySpawnerData spawner = ref spawnerRW.ValueRW;

            spawner.ElapsedTime += deltaTime;

            if (!spawner.IsEnabled) continue;

            spawner.Timer -= deltaTime;

            if (spawner.Timer > 0f) continue;


            Entity spawnEntity = spawnElements[0].EnemyPrefab; // 기본값

            for (int i = spawnElements.Length - 1; i >= 0; i--)
            {
                if (spawner.ElapsedTime >= spawnElements[i].StartTime)
                {
                    spawnEntity = spawnElements[i].EnemyPrefab;
                    break;
                }
            }
            Entity player = SystemAPI.GetSingletonEntity<PlayerTag>();
            float3 playerPos = SystemAPI.GetComponent<LocalToWorld>(player).Position;
            Random random = new Random((uint)(SystemAPI.Time.ElapsedTime * 193256) % 10000000 + 1); // 시드로 시간 사용

            int batchCount = spawner.BatchCount;

            for (int i = 0; i < batchCount; i++)
            {
                Entity newEnemy = ecb.Instantiate(spawnEntity);

                float angle = random.NextFloat(0f, math.PI * 2f);

                float3 spawnPos = playerPos + new float3(math.cos(angle), 0, math.sin(angle)) * spawner.Radius;

                ecb.AddComponent(newEnemy, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
            }

            // 다음 스폰까지의 인터벌
            spawner.Timer = spawner.SpawnInterval;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
