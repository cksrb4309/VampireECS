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
                return $"\uBE14\uB799\uD640 \uD53C\uD574\uB7C9 <v>{addValue.Damage.ToString("F1")}</v> \uC99D\uAC00";

            case BlackHoleStatType.AttackSpeed:
                return $"\uBE14\uB799\uD640 \uBC1C\uB3D9 \uC18D\uB3C4 <v>{(addValue.AttackSpeed * 100f).ToString("F0")}%</v> \uC99D\uAC00";

            case BlackHoleStatType.AcquireRadius:
                return $"\uBE14\uB799\uD640 \uD0D0\uC0C9 \uBC94\uC704 <v>{addValue.AcquireRadius.ToString("F1")}</v> \uC99D\uAC00";

            case BlackHoleStatType.Radius:
                return $"\uBE14\uB799\uD640 \uBC94\uC704 <v>{addValue.Radius.ToString("F1")}</v> \uC99D\uAC00";

            case BlackHoleStatType.Duration:
                return $"\uBE14\uB799\uD640 \uC9C0\uC18D\uC2DC\uAC04 <v>{addValue.Duration.ToString("F1")}</v>\uCD08 \uC99D\uAC00";

            case BlackHoleStatType.TickInterval:
                return $"\uBE14\uB799\uD640 \uD53C\uD574 \uAC04\uACA9 <v>{Mathf.Abs(addValue.TickInterval).ToString("F2")}</v>\uCD08 \uAC10\uC18C";

            case BlackHoleStatType.PullStrength:
                return $"\uBE14\uB799\uD640 \uD761\uC778\uB825 <v>{addValue.PullStrength.ToString("F1")}</v> \uC99D\uAC00";

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
                addValue.Damage = damageValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.AttackSpeed:
                addValue.AttackSpeed = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.AcquireRadius:
                addValue.AcquireRadius = acquireRadiusValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.Radius:
                addValue.Radius = radiusValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.Duration:
                addValue.Duration = durationValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.TickInterval:
                addValue.TickInterval = tickIntervalValue[(int)tier].GetRandomValue();
                break;

            case BlackHoleStatType.PullStrength:
                addValue.PullStrength = pullStrengthValue[(int)tier].GetRandomValue();
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
