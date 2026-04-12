using Unity.Entities;

public struct BlackHoleData : IComponentData, IAddable<BlackHoleData>
{
    public float ElapsedTime;
    public Faction OwnerFaction;

    public BlackHoleData Add(BlackHoleData other)
    {
        return this;
    }
}
