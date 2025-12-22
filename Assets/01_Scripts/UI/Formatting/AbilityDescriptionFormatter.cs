using UnityEngine;

public static class AbilityDescriptionFormatter
{
    public static string ApplyTierColor(string raw, Tier tier)
    {
        var color = TierTextColorProvider.GetColor(tier);
        string hex = ColorUtility.ToHtmlStringRGB(color);

        return raw
            .Replace("<v>", $"<color=#{hex}>")
            .Replace("</v>", "</color>");
    }
}
