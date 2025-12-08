using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSpeedConfig", menuName = "Config/Projectile/Speed")]
public class ProjectileSpeedConfig : AbilityComponentConfig<ProjectileSpeedData>
{
    public TierValue<float>[] TierValues;
    public override void Initialize() => BaseComponent.Speed = 10;
    public override void ApplyTier(Tier tier)
    {
        TierValue<float> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        float speed = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        ApplyComponent.Speed = speed;
    }
    public override void ApplyStack()
    {
        BaseComponent.Speed += ApplyComponent.Speed;
    }
    public override string GetDescription()
    {
        return $"투사체 속도 {BaseComponent.Speed.ToString()} 증가";
    }
}
