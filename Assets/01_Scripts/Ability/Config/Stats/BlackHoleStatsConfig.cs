using UnityEngine;

[CreateAssetMenu(fileName = "BlackHoleStatsConfig", menuName = "Config/Stats/BlackHoleStatsConfig")]
public class BlackHoleStatsConfig : AbilityStatsConfig<BlackHoleStatsData>
{
    [SerializeField] private TierValue<float>[] damageValue;
    [SerializeField] private TierValue<float>[] attackSpeedValue;
    [SerializeField] private TierValue<float>[] acquireRadiusValue;
    [SerializeField] private TierValue<float>[] radiusValue;
    [SerializeField] private TierValue<float>[] durationValue;
    [SerializeField] private TierValue<float>[] tickIntervalValue;
    [SerializeField] private TierValue<float>[] pullStrengthValue;

    private BlackHoleStatType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case BlackHoleStatType.Damage:
                return $"블랙홀 피해량 <v>{(addValue.DamageBonusRate * 100f).ToString("F0")}%</v> 증가";

            case BlackHoleStatType.AttackSpeed:
                return $"블랙홀 발동 속도 <v>{(addValue.AttackSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case BlackHoleStatType.AcquireRadius:
                return $"블랙홀 탐색 범위 <v>{addValue.AcquireRadiusBonus.ToString("F1")}</v> 증가";

            case BlackHoleStatType.Radius:
                return $"블랙홀 범위 <v>{addValue.RadiusBonus.ToString("F1")}</v> 증가";

            case BlackHoleStatType.Duration:
                return $"블랙홀 지속시간 <v>{addValue.DurationBonus.ToString("F1")}</v>초 증가";

            case BlackHoleStatType.TickInterval:
                return $"블랙홀 피해 간격 <v>{Mathf.Abs(addValue.TickIntervalBonus).ToString("F2")}</v>초 감소";

            case BlackHoleStatType.PullStrength:
                return $"블랙홀 흡인력 <v>{addValue.PullStrengthBonus.ToString("F1")}</v> 증가";

            default:
                return string.Empty;
        }
    }

    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;

        dataType = EnumRandom<BlackHoleStatType>.Pick();

        addValue = new BlackHoleStatsData();

        switch (dataType)
        {
            case BlackHoleStatType.Damage:
                addValue.DamageBonusRate = damageValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.AttackSpeed:
                addValue.AttackSpeedBonusRate = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.AcquireRadius:
                addValue.AcquireRadiusBonus = acquireRadiusValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.Radius:
                addValue.RadiusBonus = radiusValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.Duration:
                addValue.DurationBonus = durationValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.TickInterval:
                addValue.TickIntervalBonus = tickIntervalValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.PullStrength:
                addValue.PullStrengthBonus = pullStrengthValue[(int)tier].GetRandomValue();
                break;
        }
    }

    private enum BlackHoleStatType
    {
        Damage,
        AttackSpeed,
        AcquireRadius,
        Radius,
        Duration,
        TickInterval,
        PullStrength
    }
}
