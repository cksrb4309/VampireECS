using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileData : IComponentData
{
    public Faction OwnerFaction;

    public float3 Direction;

    public float Speed;
    public float Damage;
    public float Duration;
}

[System.Flags]
public enum Faction
{
    Player = 1 << 0,
    Enemy = 1 << 1,
}