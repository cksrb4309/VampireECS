using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityRewardGenerator : MonoBehaviour
{
    [SerializeField] List<AbilityConfig> rewardCandidates;

    [SerializeField] TimePauseController timePauseController;
    [SerializeField] RewardSelectUI ui;

    [SerializeField] AbilityConfig dummyConfig;

    Dictionary<AbilityConfig, int> abilityStacks = new();

    List<AbilityConfig> unlockedRewardPool = new();

    public void GenerateRewardChoices()
    {
        List<AbilityConfig> eligibleRewards = rewardCandidates
            .Where(cfg =>
            {
                if (abilityStacks.TryGetValue(cfg, out int currentStack))
                {
                    return !cfg.IsMaxed(currentStack);
                }
                return true;
            })
            .Union(unlockedRewardPool) // TODO : 중복된 것 제거되는지 확인바람
            .Distinct()
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
                selectedAbilities[i] = dummyConfig;
            }
        }
        ui.OnReward(selectedAbilities);
    }

    public void AcquireReward(AbilityConfig cfg)
    {
        // 최초 1회 획득만 가능하다면 제거
        if (!cfg.IsStackable && rewardCandidates.Contains(cfg))
        {
            rewardCandidates.Remove(cfg);
        }

        // 최초 획득 시
        if (!abilityStacks.ContainsKey(cfg))
        {
            // 개수 초기화
            abilityStacks[cfg] = 0;

            // Config 초기화
            cfg.Initialize();

            // 필수 선행 능력들 적용
            foreach (var preCfg in cfg.PrerequisitesAbilities)
            {
                if (!abilityStacks.ContainsKey(preCfg))
                {
                    unlockedRewardPool.Add(preCfg);

                    InitAbility(preCfg);
                }
            }
        }

        abilityStacks[cfg]++;

        cfg.ApplyToPlayer();

        timePauseController.SetPause(false);
    }
    private void InitAbility(AbilityConfig cfg)
    {   
        // 최초 1회 획득만 가능하다면 제거
        if (!cfg.IsStackable && rewardCandidates.Contains(cfg))
        {
            rewardCandidates.Remove(cfg);
        }

        // Config 초기화 및 적용
        cfg.Initialize();
        cfg.ApplyToPlayer();

        abilityStacks[cfg] = 1;

        timePauseController.SetPause(false);
    }
}
