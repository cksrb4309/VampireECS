using UnityEngine;

[CreateAssetMenu(fileName = "DummyAbilityConfig", menuName = "Config/Base/Unlock/Dummy")]
public class DummyAbilityConfig : UnlockAbilityConfig
{
    public override async void ApplyToPlayer()
    {
        // Do nothing
    }
}
