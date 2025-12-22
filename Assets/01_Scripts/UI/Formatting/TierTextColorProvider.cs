using UnityEngine;

public static class TierTextColorProvider
{
    public static TierTextColorConfig Config { get; private set; }

    public static void Initialize(TierTextColorConfig config)
    {
        Config = config;
    }

    public static Color GetColor(Tier tier)
    {
        return Config != null ? Config.GetColor(tier) : Color.white;
    }
}
