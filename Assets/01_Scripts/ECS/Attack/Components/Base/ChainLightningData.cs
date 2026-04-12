using Unity.Entities;

public struct ChainLightningData : IComponentData, IAddable<ChainLightningData>
{
    public float ElapsedTime;
    public Faction OwnerFaction;

    public ChainLightningData Add(ChainLightningData other)
    {
        return this;
    }
}
