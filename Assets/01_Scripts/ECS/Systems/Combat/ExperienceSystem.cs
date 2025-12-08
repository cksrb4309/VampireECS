using Unity.Entities;

public partial struct ExperienceSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExperienceGainEvent>();
    }
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (expEvent, entity) in
         SystemAPI.Query<ExperienceGainEvent>()
                   .WithEntityAccess())
        {
            var exp = SystemAPI.GetComponentRW<PlayerExpData>(expEvent.Target);

            exp.ValueRW.Current += expEvent.Amount;

            if (exp.ValueRW.Current >= exp.ValueRW.Required)
            {
                exp.ValueRW.Level++;
                exp.ValueRW.Current -= exp.ValueRW.Required;

                // 다음 레벨 요구 경험치 증가
                exp.ValueRW.Required = (int)(exp.ValueRW.Required * 1.3f);

                // UI 요청 이벤트 생성
                var uiEvent = ecb.CreateEntity();
                ecb.AddComponent(uiEvent, new LevelUpUIRequest());
            }

            ecb.DestroyEntity(entity); // 이벤트 소비
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
