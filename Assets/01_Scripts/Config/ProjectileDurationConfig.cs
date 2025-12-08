using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDurationConfig", menuName = "Config/Projectile/Duration")]
public class ProjectileDurationConfig : AbilityComponentConfig<ProjectileDurationData>
{
    public TierValue<float>[] TierValues;
    public override void Initialize() => BaseComponent.Duration = 1;
    public override void ApplyTier(Tier tier)
    {
        TierValue<float> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        float duration = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        ApplyComponent.Duration = duration;
    }
    public override void ApplyStack()
    {
        BaseComponent.Duration += ApplyComponent.Duration;
    }
    public override string GetDescription()
    {
        return "투사체 지속시간 증가 " + BaseComponent.Duration.ToString("F1") + "초";
    }
}
