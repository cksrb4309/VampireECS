using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityRewardController : MonoBehaviour
{
    [SerializeField] List<AbilityConfig> rewardPool;

    RewardSelectUI ui;

    Dictionary<AbilityConfig, int> abilityStacks = new();

    private void Start()
    {
        ui = GetComponent<RewardSelectUI>();
    }
    public void GenerateRewardChoices()
    {
        List<AbilityConfig> eligibleRewards = rewardPool
            .Where(cfg =>
            {
                abilityStacks.TryGetValue(cfg, out int currentStack);
                return !cfg.IsMaxed(currentStack);
            })
            .ToList();

        AbilityConfig[] selectedAbilities = new AbilityConfig[3];

        for (int i = 0; i < 3; i++)
        {
            if (eligibleRewards.Count > 0)
            {
                int index = Random.Range(0, eligibleRewards.Count);

                selectedAbilities[i] = eligibleRewards[index];

                float r = Random.value;
                Tier tier = r >= 0.75f ? Tier.Gold :
                            r >= 0.45f ? Tier.Silver : Tier.Bronze;

                selectedAbilities[i].ApplyTier(tier);

                eligibleRewards.RemoveAt(index);
            }
            else
            {
                selectedAbilities[i] = null;
            }
        }
        ui.OnReward(selectedAbilities);
    }

    public void AcquireReward(AbilityConfig cfg)
    {
        if (!abilityStacks.ContainsKey(cfg))
            abilityStacks[cfg] = 0;

        abilityStacks[cfg]++;
    }
}
