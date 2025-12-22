using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/UI/Tier Text Color")]
public class TierTextColorConfig : ScriptableObject
{
    [System.Serializable]
    public struct TierColor
    {
        public Tier tier;
        public Color color;
    }

    [SerializeField] private TierColor[] tierColors;

    private Dictionary<Tier, Color> _cache;

    public Color GetColor(Tier tier)
    {
        _cache ??= BuildCache();
        return _cache.TryGetValue(tier, out var c) ? c : Color.white;
    }

    private Dictionary<Tier, Color> BuildCache()
    {
        var dict = new Dictionary<Tier, Color>();

        foreach (var entry in tierColors)
        {
            dict[entry.tier] = entry.color;
        }
        return dict;
    }
}
