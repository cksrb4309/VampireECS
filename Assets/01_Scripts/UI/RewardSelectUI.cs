using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.STP;

public class RewardSelectUI : MonoBehaviour
{
    [SerializeField] RewardCardPanel[] rewardCardPanels;

    [SerializeField] AbilityRewardGenerator rewardManager;

    public void OnReward(AbilityConfig[] abilityConfigs)
    {
        if (abilityConfigs == null || rewardCardPanels == null)
            return;

        SetVisible(true);

        rewardCardPanels
            .Zip(abilityConfigs, (panel, cfg) => (panel, cfg))
            .ToList()
            .ForEach(x =>
            {
                var panel = x.panel;
                var cfg = x.cfg;

                string raw = cfg.GetDescription();
                string colored = AbilityDescriptionFormatter.ApplyTierColor(raw, cfg.CurrentTier);

                panel.Description.text = colored;

                panel.Icon.sprite = cfg.Icon;

                panel.Button.onClick.RemoveAllListeners();
                panel.Button.onClick.AddListener(() => OnSelectReward(cfg));

                panel.Button.gameObject.SetActive(true);
            });

        // 패널이 더 많은 경우 나머지 비활성화
        for (int i = abilityConfigs.Length; i < rewardCardPanels.Length; i++)
            rewardCardPanels[i].Button?.gameObject.SetActive(false);
    }

    void SetVisible(bool isVisible)
    {
        for (int i = 0; i < rewardCardPanels.Length; i++)
            rewardCardPanels[i].Button.gameObject.SetActive(isVisible);
    }

    void OnSelectReward(AbilityConfig cfg)
    {
        SetVisible(false);

        rewardManager.AcquireReward(cfg);
    }
}

[System.Serializable]
public class RewardCardPanel
{
    public TMP_Text Description;
    public Image Icon;
    public Button Button;
}
