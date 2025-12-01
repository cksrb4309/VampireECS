using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct RenderTrailJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([EntityIndexInQuery] int index,
                 ref RenderTrailData trailData,
                 ref DynamicRotationData rotationData,
                 ref LocalToWorld transform)
    {
        if (math.distance(transform.Position, trailData.LastSpawnPos) > trailData.SpawnDistance)
        {
            Entity trailEntity = ECB.Instantiate(index, trailData.Prefab);

            ECB.SetComponent(index, trailEntity, new LocalTransform
            {
                Position = transform.Position,
                Rotation = transform.Rotation,
                Scale = trailData.KeyScales[0]
            });

            ECB.SetComponent(index, trailEntity, new RotationData 
            {
                Axis = rotationData.CurrentAxis,
                Speed = rotationData.RotationSpeed
            });

            ECB.AddComponent(index, trailEntity, new ShrinkOverTimeData
            {
                Lifetime = trailData.LifeTime,
                ElapsedTime = 0f,
                KeyTimes = trailData.KeyTimes,
                KeyScales = trailData.KeyScales
            });

            // 마지막 위치 갱신
            trailData.LastSpawnPos = transform.Position;
        }
    }
}
