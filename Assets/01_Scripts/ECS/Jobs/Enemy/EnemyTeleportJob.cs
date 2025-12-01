using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyTeleportJob : IJobEntity
{
    public float Radius;
    public float Distance;
    public float3 PlayerPos;
    public Random Random;
    void Execute([EntityIndexInQuery] int index,
               ref EnemyMoveData enemyMoveData,
               ref LocalTransform transform)
    {
        if (math.distance(transform.Position, PlayerPos) > Distance)
        {
            float angle = Random.NextFloat(0f, math.PI * 2f);

            float3 spawnPos = PlayerPos + new float3(math.cos(angle), 0, math.sin(angle)) * Radius;

            transform.Position = spawnPos;
        }
    }
}
