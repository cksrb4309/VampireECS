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
                return $"연쇄 번개 피해량 <v>{(addValue.Damage * 100f).ToString("F0")}%</v> 증가";

            case ChainLightningStatType.AttackSpeed:
                return $"연쇄 번개 발동 속도 <v>{(addValue.AttackSpeed * 100f).ToString("F0")}%</v> 증가";

            case ChainLightningStatType.AcquireRadius:
                return $"연쇄 번개 첫 탐색 범위 <v>{addValue.AcquireRadius.ToString("F1")}</v> 증가";

            case ChainLightningStatType.JumpRadius:
                return $"연쇄 번개 점프 범위 <v>{addValue.JumpRadius.ToString("F1")}</v> 증가";

            case ChainLightningStatType.MaxTargets:
                return $"연쇄 번개 타격 대상 수 <v>{addValue.MaxTargets}</v> 증가";

            case ChainLightningStatType.DamageMultiplierPerJump:
                return $"연쇄 번개 점프 피해 유지율 <v>{(addValue.DamageMultiplierPerJump * 100f).ToString("F0")}%</v> 증가";

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
                addValue.Damage = damageValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.AttackSpeed:
                addValue.AttackSpeed = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.AcquireRadius:
                addValue.AcquireRadius = acquireRadiusValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.JumpRadius:
                addValue.JumpRadius = jumpRadiusValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.MaxTargets:
                addValue.MaxTargets = maxTargetsValue[(int)tier].GetRandomValue();
                break;

            case ChainLightningStatType.DamageMultiplierPerJump:
                addValue.DamageMultiplierPerJump = damageMultiplierPerJumpValue[(int)tier].GetRandomValue();
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
