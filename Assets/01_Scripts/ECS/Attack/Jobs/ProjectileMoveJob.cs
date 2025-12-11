using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct ProjectileMoveJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([EntityIndexInQuery] int index,
                 Entity entity,
                 ref ProjectileData projectile,
                 ref LocalTransform transform)
    {
        // 이동
        transform.Position += projectile.Direction * projectile.Speed * DeltaTime;

        // 수명 감소
        projectile.Duration -= DeltaTime;

        // 제거
        if (projectile.Duration <= 0f)
        {
            ECB.DestroyEntity(index, entity);
        }
    }
}
