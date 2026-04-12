using UnityEngine;

[CreateAssetMenu(fileName = "MeteorStrikeStatsConfig", menuName = "Config/Stats/MeteorStrikeStatsConfig")]
public class MeteorStrikeStatsConfig : AbilityStatsConfig<MeteorStrikeStatsData>
{
    [SerializeField] private TierValue<float>[] damageValue;
    [SerializeField] private TierValue<float>[] attackSpeedValue;
    [SerializeField] private TierValue<float>[] acquireRadiusValue;
    [SerializeField] private TierValue<float>[] impactRadiusValue;
    [SerializeField] private TierValue<float>[] impactDelayValue;
    [SerializeField] private TierValue<int>[] meteorCountValue;
    [SerializeField] private TierValue<float>[] scatterRadiusValue;

    private MeteorStrikeStatType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case MeteorStrikeStatType.Damage:
                return $"메테오 피해량 <v>{(addValue.DamageBonusRate * 100f).ToString("F0")}%</v> 증가";

            case MeteorStrikeStatType.AttackSpeed:
                return $"메테오 발동 속도 <v>{(addValue.AttackSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case MeteorStrikeStatType.AcquireRadius:
                return $"메테오 탐색 범위 <v>{addValue.AcquireRadiusBonus.ToString("F1")}</v> 증가";

            case MeteorStrikeStatType.ImpactRadius:
                return $"메테오 폭발 범위 <v>{addValue.ImpactRadiusBonus.ToString("F1")}</v> 증가";

            case MeteorStrikeStatType.ImpactDelay:
                return $"메테오 낙하 시간 <v>{Mathf.Abs(addValue.ImpactDelayBonus).ToString("F1")}</v>초 감소";

            case MeteorStrikeStatType.MeteorCount:
                return $"메테오 개수 <v>{addValue.MeteorCountBonus}</v>개 증가";

            case MeteorStrikeStatType.ScatterRadius:
                return $"메테오 분산 범위 <v>{addValue.ScatterRadiusBonus.ToString("F1")}</v> 증가";

            default:
                return string.Empty;
        }
    }

    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;

        dataType = EnumRandom<MeteorStrikeStatType>.Pick();

        addValue = new MeteorStrikeStatsData();

        switch (dataType)
        {
            case MeteorStrikeStatType.Damage:
                addValue.DamageBonusRate = damageValue[(int)tier].GetRandomValue();
                break;

            case MeteorStrikeStatType.AttackSpeed:
                addValue.AttackSpeedBonusRate = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case MeteorStrikeStatType.AcquireRadius:
                addValue.AcquireRadiusBonus = acquireRadiusValue[(int)tier].GetRandomValue();
                break;

            case MeteorStrikeStatType.ImpactRadius:
                addValue.ImpactRadiusBonus = impactRadiusValue[(int)tier].GetRandomValue();
                break;

            case MeteorStrikeStatType.ImpactDelay:
                addValue.ImpactDelayBonus = impactDelayValue[(int)tier].GetRandomValue();
                break;

            case MeteorStrikeStatType.MeteorCount:
                addValue.MeteorCountBonus = meteorCountValue[(int)tier].GetRandomValue();
                break;

            case MeteorStrikeStatType.ScatterRadius:
                addValue.ScatterRadiusBonus = scatterRadiusValue[(int)tier].GetRandomValue();
                break;
        }
    }

    private enum MeteorStrikeStatType
    {
        Damage,
        AttackSpeed,
        AcquireRadius,
        ImpactRadius,
        ImpactDelay,
        MeteorCount,
        ScatterRadius
    }
}
