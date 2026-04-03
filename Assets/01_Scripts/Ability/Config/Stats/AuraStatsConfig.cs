using UnityEngine;

[CreateAssetMenu(fileName = "AuraStatsConfig", menuName = "Config/Stats/AuraStatsConfig")]
public class AuraStatsConfig : AbilityStatsConfig<AuraStatsData>
{
    [SerializeField] TierValue<float>[] damageValue;
    [SerializeField] TierValue<float>[] attackSpeedValue;
    [SerializeField] TierValue<float>[] radiusValue;

    AuraStatDataType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case AuraStatDataType.Damage:
                return $"장판 공격력 <v>{(addValue.DamageBonusRate * 100f).ToString("F0")}%</v> 증가";

            case AuraStatDataType.AttackSpeed:
                return $"장판 공격 속도 <v>{(addValue.AttackSpeedBonusRate * 100f).ToString("F0")}%</v> 증가";

            case AuraStatDataType.Radius:
                return $"장판 범위 <v>{(addValue.RadiusBonusRate * 100f).ToString("F0")}%</v> 증가";

            default:
                return string.Empty;
        }
    }

    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;

        dataType = EnumRandom<AuraStatDataType>.Pick();

        addValue = new AuraStatsData();

        switch (dataType)
        {
            case AuraStatDataType.Damage:
                addValue.DamageBonusRate = damageValue[(int)tier].GetRandomValue();
                break;

            case AuraStatDataType.AttackSpeed:
                addValue.AttackSpeedBonusRate = attackSpeedValue[(int)tier].GetRandomValue();
                break;

            case AuraStatDataType.Radius:
                addValue.RadiusBonusRate = radiusValue[(int)tier].GetRandomValue();
                break;
        }
    }

    enum AuraStatDataType
    {
        Damage,
        AttackSpeed,
        Radius
    }
}
