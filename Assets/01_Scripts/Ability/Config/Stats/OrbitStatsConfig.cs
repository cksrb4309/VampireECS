using UnityEngine;

[CreateAssetMenu(fileName = "OrbitStatsConfig", menuName = "Config/Stats/OrbitStatsConfig")]
public class OrbitStatsConfig : AbilityStatsConfig<OrbitStatsData>
{
    [SerializeField] private TierValue<float>[] damageValue;
    [SerializeField] private TierValue<float>[] attackSpeedValue;
    [SerializeField] private TierValue<float>[] orbitSpeedValue;
    [SerializeField] private TierValue<float>[] orbitRadiusValue;
    [SerializeField] private TierValue<float>[] hitRadiusValue;
    [SerializeField] private TierValue<int>[] projectileCountValue;
    [SerializeField] private TierValue<float>[] hitCooldownValue;

    private OrbitStatType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case OrbitStatType.Damage:
                return $"궤도체 피해량 <v>{(addValue.DamageBonusRate * 100f).ToString("F0")}%</v> 증가";

            case OrbitStatType.AttackSpeed:
                return $"궤도체 재소환 속도 <v>{(addValue.AttackSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case OrbitStatType.OrbitSpeed:
                return $"궤도체 회전 속도 <v>{(addValue.OrbitSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case OrbitStatType.OrbitRadius:
                return $"궤도체 궤도 반경 <v>{addValue.OrbitRadiusBonus.ToString("F1")}</v> 증가";

            case OrbitStatType.HitRadius:
                return $"궤도체 타격 범위 <v>{addValue.HitRadiusBonus.ToString("F1")}</v> 증가";

            case OrbitStatType.ProjectileCount:
                return $"궤도체 개수 <v>{addValue.ProjectileCountBonus}</v>개 증가";

            case OrbitStatType.HitCooldown:
                return $"궤도체 재타격 간격 <v>{Mathf.Abs(addValue.HitCooldownBonus).ToString("F2")}</v>초 감소";

            default:
                return string.Empty;
        }
    }

    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;
        dataType = EnumRandom<OrbitStatType>.Pick();
        addValue = new OrbitStatsData();

        switch (dataType)
        {
            case OrbitStatType.Damage:
                addValue.DamageBonusRate = damageValue[(int)tier].GetRandomValue();
                break;

            case OrbitStatType.AttackSpeed:
                addValue.AttackSpeedBonusRate = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case OrbitStatType.OrbitSpeed:
                addValue.OrbitSpeedBonusRate = orbitSpeedValue[(int)tier].GetRandomValue();
                break;

            case OrbitStatType.OrbitRadius:
                addValue.OrbitRadiusBonus = orbitRadiusValue[(int)tier].GetRandomValue();
                break;

            case OrbitStatType.HitRadius:
                addValue.HitRadiusBonus = hitRadiusValue[(int)tier].GetRandomValue();
                break;

            case OrbitStatType.ProjectileCount:
                addValue.ProjectileCountBonus = projectileCountValue[(int)tier].GetRandomValue();
                break;

            case OrbitStatType.HitCooldown:
                addValue.HitCooldownBonus = hitCooldownValue[(int)tier].GetRandomValue();
                break;
        }
    }

    private enum OrbitStatType
    {
        Damage,
        AttackSpeed,
        OrbitSpeed,
        OrbitRadius,
        HitRadius,
        ProjectileCount,
        HitCooldown
    }
}
