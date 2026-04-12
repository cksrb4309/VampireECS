using Unity.Entities;

public struct ChainLightningBaseStatsData : IComponentData
{
    public float BaseDamage;
    public float BaseAttackSpeed;
    public float BaseAcquireRadius;
    public float BaseJumpRadius;
    public int BaseMaxTargets;
    public float BaseDamageMultiplierPerJump;
}
