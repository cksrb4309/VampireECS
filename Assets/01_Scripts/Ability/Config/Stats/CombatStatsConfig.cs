using UnityEngine;

[CreateAssetMenu(fileName = "CombatStatsConfig", menuName = "Config/Stats/CombatStatsConfig")]
public class CombatStatsConfig : AbilityStatsConfig<CombatStatsData>
{
    [SerializeField] TierValue<float>[] damageValue;
    [SerializeField] TierValue<float>[] attackSpeedValue;
    [SerializeField] TierValue<float>[] attackRangeValue;

    CombatStatsDataType dataType;

    public override string GetDescription()
    {
        switch (dataType)
        {
            case CombatStatsDataType.Damage:
                return $"기본 공격력 <v>{(addValue.Damage * 100f).ToString("F0")}%</v> 증가";

            case CombatStatsDataType.AttackSpeed:
                return $"기본 공격 속도 <v>{(addValue.AttackSpeed * 100f).ToString("F0")}%</v> 증가";

            case CombatStatsDataType.AttackRange:
                return $"기본 공격 범위 <v>{(addValue.AttackRange * 100f).ToString("F0")}%</v> 증가";

            default:
                return string.Empty;
        }
    }
    public override void ApplyTier(Tier tier)
    {
        CurrentTier = tier;

        dataType = EnumRandom<CombatStatsDataType>.Pick();

        addValue = new CombatStatsData();

        switch (dataType)
        {
            case CombatStatsDataType.Damage:
                addValue.Damage = damageValue[(int)tier].GetRandomValue(); break;

            case CombatStatsDataType.AttackSpeed:
                addValue.AttackSpeed = attackSpeedValue[(int)tier].GetRandomValue(); break;

            case CombatStatsDataType.AttackRange:
                addValue.AttackRange = attackRangeValue[(int)tier].GetRandomValue(); break;
        }
    }
    enum CombatStatsDataType
    {
        Damage,
        AttackSpeed,
        AttackRange
    }
}
