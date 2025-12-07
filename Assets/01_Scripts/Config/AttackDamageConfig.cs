using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDamageConfig", menuName = "Config/Stats/AttackDamage")]
public class AttackDamageConfig : AbilityComponentConfig<AttackDamageData>
{
    public TierValue<float>[] TierValues;
    public override void ApplyTier(Tier tier)
    {
        TierValue<float> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        float damage = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        BaseComponent.Damage = damage;
    }
    public override void ApplyStack(ref AttackDamageData abilityConfig)
    {
        abilityConfig.Damage += BaseComponent.Damage;
    }
    public override string GetDescription()
    {
        return $"공격 데미지 {BaseComponent.Damage.ToString()} 증가";
    }
}
