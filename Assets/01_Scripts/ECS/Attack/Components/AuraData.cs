using Unity.Entities;

public struct AuraData : IComponentData, IAddable<AuraData>
{
    public float ElapsedTime;

    public Faction OwnerFaction;

    public AuraData Add(AuraData other) => new AuraData();
}
