using UnityEngine;

[CreateAssetMenu(fileName = "ShooterStatsConfig", menuName = "Config/Stats/ShooterStatsConfig")]
public class ShooterStatsConfig : AbilityStatsConfig<ShooterStatsData>
{
    [SerializeField] TierValue<float>[] damageValue;
    [SerializeField] TierValue<float>[] attackSpeedValue;
    [SerializeField] TierValue<float>[] projectileSpeedValue;
    [SerializeField] TierValue<float>[] projectileDurationValue;
    [SerializeField] TierValue<int>[] projectileCountValue;

    private ShooterStatDataType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case ShooterStatDataType.Damage:
                return $"에너지볼 공격력 <v>{(addValue.DamageBonusRate * 100f).ToString("F0")}%</v> 증가";

            case ShooterStatDataType.AttackSpeed:
                return $"에너지볼 공격 속도 <v>{(addValue.AttackSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case ShooterStatDataType.ProjectileSpeed:
                return $"에너지볼 속도 <v>{(addValue.ProjectileSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case ShooterStatDataType.ProjectileDuration:
                return $"에너지볼 지속시간 <v>{addValue.ProjectileDurationBonus.ToString("F1")}</v>초 증가";

            case ShooterStatDataType.ProjectileCount:
                return $"에너지볼 개수 <v>{addValue.ProjectileCountBonus}</v>개 증가";

            default:
                return string.Empty;
        }
    }

    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;

        dataType = EnumRandom<ShooterStatDataType>.Pick();

        addValue = new ShooterStatsData();

        switch (dataType)
        {
            case ShooterStatDataType.Damage:
                addValue.DamageBonusRate = damageValue[(int)tier].GetRandomValue();
                break;

            case ShooterStatDataType.AttackSpeed:
                addValue.AttackSpeedBonusRate = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case ShooterStatDataType.ProjectileSpeed:
                addValue.ProjectileSpeedBonusRate = projectileSpeedValue[(int)tier].GetRandomValue();
                break;

            case ShooterStatDataType.ProjectileDuration:
                addValue.ProjectileDurationBonus = projectileDurationValue[(int)tier].GetRandomValue();
                break;

            case ShooterStatDataType.ProjectileCount:
                addValue.ProjectileCountBonus = projectileCountValue[(int)tier].GetRandomValue();
                break;
        }
    }

    enum ShooterStatDataType
    {
        Damage,
        AttackSpeed,
        ProjectileSpeed,
        ProjectileDuration,
        ProjectileCount
    }
}
