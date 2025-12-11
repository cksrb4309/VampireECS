using Unity.Entities;

public struct DamageEventData : IComponentData
{
    public Entity Target;
    public float Damage;
}
