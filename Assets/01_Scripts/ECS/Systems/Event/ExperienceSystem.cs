using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct ExperienceSystem : ISystem
{
    private EntityQuery _gainQuery;

    public void OnCreate(ref SystemState state)
    {
        _gainQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<ExperienceGainEvent>()
        );
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // GainEvent 모두 미리 가져오기 (딱 1번)
        var gainEvents = _gainQuery.ToEntityArray(state.WorldUpdateAllocator);
        var gainDatas = _gainQuery.ToComponentDataArray<ExperienceGainEvent>(state.WorldUpdateAllocator);

        // 플레이어 경험치 처리
        foreach (var (exp, playerEntity) in
            SystemAPI.Query<RefRW<PlayerExpData>>().WithEntityAccess())
        {
            // 경험치 누적
            for (int i = 0; i < gainDatas.Length; i++)
                exp.ValueRW.Current += gainDatas[i].Amount;

            // 레벨업 처리
            while (exp.ValueRW.Current >= exp.ValueRW.Required)
            {
                exp.ValueRW.Current -= exp.ValueRW.Required;
                exp.ValueRW.Required = (int)(exp.ValueRW.Required * 1.3f);
                exp.ValueRW.Level++;

                ecb.AddComponent<LevelUpUIRequest>(ecb.CreateEntity());
            }
        }

        // GainEvent 제거
        for (int i = 0; i < gainEvents.Length; i++)
        {
            ecb.DestroyEntity(gainEvents[i]);
        }

        // 반영
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
