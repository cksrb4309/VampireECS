using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(DestructionCleanupSystemGroup))]
public partial struct BlackHolePresentationSystem : ISystem
{
    private EntityQuery fieldQuery;

    public void OnCreate(ref SystemState state)
    {
        fieldQuery = state.GetEntityQuery(ComponentType.ReadOnly<BlackHoleFieldData>());
    }

    public void OnUpdate(ref SystemState state)
    {
        if (fieldQuery.IsEmptyIgnoreFilter && !BlackHoleViewManager.HasInstance)
        {
            return;
        }

        BlackHoleViewManager viewManager = BlackHoleViewManager.Instance;
        viewManager.BeginFrameSync();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (fieldRO, entity) in
            SystemAPI.Query<RefRO<BlackHoleFieldData>>()
                .WithNone<BlackHoleVisualID>()
                .WithEntityAccess())
        {
            int viewId = viewManager.CreateView();
            BlackHoleFieldData field = fieldRO.ValueRO;

            ecb.AddComponent(entity, new BlackHoleVisualID
            {
                Value = viewId
            });

            viewManager.UpdateView(
                viewId,
                field.Position,
                field.Radius,
                field.RemainingDuration,
                field.TotalDuration,
                field.PullStrength);
        }

        foreach (var (fieldRO, visualIdRO) in
            SystemAPI.Query<RefRO<BlackHoleFieldData>, RefRO<BlackHoleVisualID>>())
        {
            BlackHoleFieldData field = fieldRO.ValueRO;

            viewManager.UpdateView(
                visualIdRO.ValueRO.Value,
                field.Position,
                field.Radius,
                field.RemainingDuration,
                field.TotalDuration,
                field.PullStrength);
        }

        viewManager.EndFrameSync();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
