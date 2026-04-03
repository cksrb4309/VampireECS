using UnityEngine;

[CreateAssetMenu(fileName = "ChainLightningStatsConfig", menuName = "Config/Stats/ChainLightningStatsConfig")]
public class ChainLightningStatsConfig : AbilityStatsConfig<ChainLightningStatsData>
{
    [SerializeField] private TierValue<float>[] damageValue;
    [SerializeField] private TierValue<float>[] attackSpeedValue;
    [SerializeField] private TierValue<float>[] acquireRadiusValue;
    [SerializeField] private TierValue<float>[] jumpRadiusValue;
    [SerializeField] private TierValue<int>[] maxTargetsValue;
    [SerializeField] private TierValue<float>[] damageMultiplierPerJumpValue;

    private ChainLightningStatType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case ChainLightningStatType.Damage:
                return $"연쇄 번개 피해량 <v>{(addValue.DamageBonusRate * 100f).ToString("F0")}%</v> 증가";

            case ChainLightningStatType.AttackSpeed:
                return $"연쇄 번개 발동 속도 <v>{(addValue.AttackSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case ChainLightningStatType.AcquireRadius:
                return $"연쇄 번개 첫 탐색 범위 <v>{addValue.AcquireRadiusBonus.ToString("F1")}</v> 증가";

            case ChainLightningStatType.JumpRadius:
                return $"연쇄 번개 점프 범위 <v>{addValue.JumpRadiusBonus.ToString("F1")}</v> 증가";

            case ChainLightningStatType.MaxTargets:
                return $"연쇄 번개 타격 대상 수 <v>{addValue.MaxTargetsBonus}</v> 증가";

            case ChainLightningStatType.DamageMultiplierPerJump:
                return $"연쇄 번개 점프 피해 유지율 <v>{(addValue.DamageMultiplierPerJumpBonus * 100f).ToString("F0")}%</v> 증가";

            default:
                return string.Empty;
        }
    }

    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;

        dataType = EnumRandom<ChainLightningStatType>.Pick();

        addValue = new ChainLightningStatsData();

        switch (dataType)
        {
            case ChainLightningStatType.Damage:
                addValue.DamageBonusRate = damageValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.AttackSpeed:
                addValue.AttackSpeedBonusRate = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.AcquireRadius:
                addValue.AcquireRadiusBonus = acquireRadiusValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.JumpRadius:
                addValue.JumpRadiusBonus = jumpRadiusValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.MaxTargets:
                addValue.MaxTargetsBonus = maxTargetsValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.DamageMultiplierPerJump:
                addValue.DamageMultiplierPerJumpBonus = damageMultiplierPerJumpValue[(int)tier].GetRandomValue();
                break;
        }
    }

    private enum ChainLightningStatType
    {
        Damage,
        AttackSpeed,
        AcquireRadius,
        JumpRadius,
        MaxTargets,
        DamageMultiplierPerJump
    }
}
