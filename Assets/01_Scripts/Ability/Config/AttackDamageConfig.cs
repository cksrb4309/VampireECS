using System.Linq;
using Unity.Physics;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDamageConfig", menuName = "Config/Stats/AttackDamage")]
public class AttackDamageConfig : AbilityComponentConfig<AttackDamageData>
{
    public TierValue<float>[] TierValues;
    public override void Initialize() => BaseValue = new AttackDamageData { Damage = 5f };
    public override void ApplyTier(Tier tier)
    {
        TierValue<float> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        float damage = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        AppliedValue.Damage = damage;
    }
    public override string GetDescription()
    {
        return $"공격 데미지 {AppliedValue.Damage.ToString("F1")} 증가";
    }
}
