using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(DamageSetupSystemGroup))]
partial struct AuraRenderSystem : ISystem
{
    private NativeList<AuraSnapshot> snapshotBuffer;

    public void OnCreate(ref SystemState state)
    {
        snapshotBuffer = new NativeList<AuraSnapshot>(Allocator.Persistent);

        state.RequireForUpdate<AuraData>();
    }
    public void OnDestroy(ref SystemState state)
    {
        if (snapshotBuffer.IsCreated)
        {
            snapshotBuffer.Dispose();
        }
    }
    public void OnUpdate(ref SystemState state)
    {
        snapshotBuffer.Clear();

        var job = new AuraSnapshotJob
        {
            Output = snapshotBuffer.AsParallelWriter()
        };

        var handle = job.ScheduleParallel(state.Dependency);
        handle.Complete();
        

        for (int i = 0; i < snapshotBuffer.Length; i++)
        {
            var snap = snapshotBuffer[i];
            var view = AuraViewManager.Instance.GetView(snap.ViewID);

            view.SetPosition(snap.Position);
            view.SetRadius(snap.Radius);
        }

        state.Dependency = handle;
    }
}
public struct AuraSnapshot
{
    public float3 Position;
    public float Radius;
    public int ViewID;
}
