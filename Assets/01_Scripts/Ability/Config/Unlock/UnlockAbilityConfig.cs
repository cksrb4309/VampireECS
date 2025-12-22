
using UnityEngine;

public abstract class UnlockAbilityConfig : AbilityConfig
{
    [SerializeField] Tier fixedTier;

    [TextArea, SerializeField] private string description;
    public override void ApplyTier(Tier tier)
    {
        CurrentTier = fixedTier;
    }
    public override string GetDescription() => description;
}
