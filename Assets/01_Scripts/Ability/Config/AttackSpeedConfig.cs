using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "AttackSpeedConfig", menuName = "Config/Stats/AttackSpeed")]
public class AttackSpeedConfig : AbilityComponentConfig<AttackSpeedData>
{
    public TierValue<float>[] TierValues;
    public override void Initialize() => BaseValue = new AttackSpeedData { AttackSpeed = 5f };
    public override void ApplyTier(Tier tier)
    {
        TierValue<float> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        float attackSpeed = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        AppliedValue.AttackSpeed = attackSpeed;
    }
    public override string GetDescription()
    {
        return $"공격속도 { AppliedValue.AttackSpeed.ToString("F1") } 증가";
    }
}