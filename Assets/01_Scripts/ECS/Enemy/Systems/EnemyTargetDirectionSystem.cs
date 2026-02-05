using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyTargetDirectionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPos = SystemAPI.GetComponentRO<LocalTransform>(SystemAPI.GetSingletonEntity<PlayerTag>()).ValueRO.Position;
        
        foreach (var (transformRO, shooterDataRW) in SystemAPI.Query<
            RefRO<LocalTransform>,
            RefRW<ShooterData>>()
            .WithAll<EnemyMoveData>())
        {
            ref readonly LocalTransform transform = ref transformRO.ValueRO;
            ref ShooterData shooterData = ref shooterDataRW.ValueRW;

            shooterData.Direction = math.normalize(playerPos - transform.Position);
        }
    }
}