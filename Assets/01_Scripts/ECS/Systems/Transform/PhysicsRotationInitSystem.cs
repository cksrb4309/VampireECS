using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct PhysicsRotationInitSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (mass, entity)
            in SystemAPI.Query<RefRW<PhysicsMass>>()
                .WithAll<PhysicsInit>()
                .WithEntityAccess())
        {
            mass.ValueRW.InverseInertia = float3.zero;

            ecb.RemoveComponent<PhysicsInit>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}