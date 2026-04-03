using Unity.Entities;

public struct MeteorStrikeData : IComponentData, IAddable<MeteorStrikeData>
{
    public float ElapsedTime;
    public Faction OwnerFaction;

    public MeteorStrikeData Add(MeteorStrikeData other)
    {
        return this;
    }
}
