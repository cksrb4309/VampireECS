using UnityEngine;

[CreateAssetMenu(fileName = "BoomerangStatsConfig", menuName = "Config/Stats/BoomerangStatsConfig")]
public class BoomerangStatsConfig : AbilityStatsConfig<BoomerangStatsData>
{
    [SerializeField] private TierValue<float>[] damageValue;
    [SerializeField] private TierValue<float>[] attackSpeedValue;
    [SerializeField] private TierValue<float>[] speedValue;
    [SerializeField] private TierValue<float>[] maxDistanceValue;
    [SerializeField] private TierValue<float>[] hitRadiusValue;
    [SerializeField] private TierValue<int>[] projectileCountValue;
    [SerializeField] private TierValue<float>[] hitCooldownValue;

    private BoomerangStatType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case BoomerangStatType.Damage:
                return $"부메랑 피해량 <v>{(addValue.DamageBonusRate * 100f).ToString("F0")}%</v> 증가";

            case BoomerangStatType.AttackSpeed:
                return $"부메랑 발동 속도 <v>{(addValue.AttackSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case BoomerangStatType.Speed:
                return $"부메랑 비행 속도 <v>{(addValue.SpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case BoomerangStatType.MaxDistance:
                return $"부메랑 비행 거리 <v>{addValue.MaxDistanceBonus.ToString("F1")}</v> 증가";

            case BoomerangStatType.HitRadius:
                return $"부메랑 타격 범위 <v>{addValue.HitRadiusBonus.ToString("F1")}</v> 증가";

            case BoomerangStatType.ProjectileCount:
                return $"부메랑 개수 <v>{addValue.ProjectileCountBonus}</v>개 증가";

            case BoomerangStatType.HitCooldown:
                return $"부메랑 재타격 간격 <v>{Mathf.Abs(addValue.HitCooldownBonus).ToString("F2")}</v>초 감소";

            default:
                return string.Empty;
        }
    }

    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;

        dataType = EnumRandom<BoomerangStatType>.Pick();

        addValue = new BoomerangStatsData();

        switch (dataType)
        {
            case BoomerangStatType.Damage:
                addValue.DamageBonusRate = damageValue[(int)tier].GetRandomValue();
                break;

            case BoomerangStatType.AttackSpeed:
                addValue.AttackSpeedBonusRate = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case BoomerangStatType.Speed:
                addValue.SpeedBonusRate = speedValue[(int)tier].GetRandomValue();
                break;

            case BoomerangStatType.MaxDistance:
                addValue.MaxDistanceBonus = maxDistanceValue[(int)tier].GetRandomValue();
                break;

            case BoomerangStatType.HitRadius:
                addValue.HitRadiusBonus = hitRadiusValue[(int)tier].GetRandomValue();
                break;

            case BoomerangStatType.ProjectileCount:
                addValue.ProjectileCountBonus = projectileCountValue[(int)tier].GetRandomValue();
                break;

            case BoomerangStatType.HitCooldown:
                addValue.HitCooldownBonus = hitCooldownValue[(int)tier].GetRandomValue();
                break;
        }
    }

    private enum BoomerangStatType
    {
        Damage,
        AttackSpeed,
        Speed,
        MaxDistance,
        HitRadius,
        ProjectileCount,
        HitCooldown
    }
}
