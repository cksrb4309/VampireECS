using Unity.Entities;

[InternalBufferCapacity(8)]
public struct BoomerangRecentHitData : IBufferElementData
{
    public Entity Target;
    public float RemainingCooldown;
}
