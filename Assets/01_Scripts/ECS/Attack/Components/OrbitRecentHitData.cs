using Unity.Entities;

public struct OrbitRecentHitData : IBufferElementData
{
    public Entity Target;
    public float RemainingCooldown;
}
