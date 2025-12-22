using Unity.Entities;

public abstract class AbilityStatsConfig<TStats> : AbilityConfig
    where TStats : unmanaged, IComponentData, IAddable<TStats>, IInitializableStats<TStats>
{
    protected TStats baseValue;
    protected TStats addValue;
    public override void ApplyToPlayer()
    {
        baseValue = baseValue.Add(addValue);

        PlayerStatApplier<TStats>.ApplyStats(baseValue);
    }
    public override void Initialize()
    {
        baseValue.Initialize();
    }
}
