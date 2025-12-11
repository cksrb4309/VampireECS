using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDurationConfig", menuName = "Config/Projectile/Duration")]
public class ProjectileDurationConfig : AbilityComponentConfig<ProjectileDurationData>
{
    public TierValue<float>[] TierValues;
    public override void Initialize() => BaseValue = new ProjectileDurationData { Duration = 1f };
    public override void ApplyTier(Tier tier)
    {
        TierValue<float> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        float duration = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        AppliedValue.Duration = duration;
    }
    public override string GetDescription()
    {
        return "투사체 지속시간 증가 " + AppliedValue.Duration.ToString("F1") + "초";
    }
}
