using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSpeedConfig", menuName = "Config/Projectile/Speed")]
public class ProjectileSpeedConfig : AbilityComponentConfig<ProjectileSpeedData>
{
    public TierValue<float>[] TierValues;
    public override void Initialize() => BaseValue = new ProjectileSpeedData { Speed = 10 };
    public override void ApplyTier(Tier tier)
    {
        TierValue<float> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        float speed = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        AppliedValue.Speed = speed;
    }
    public override string GetDescription()
    {
        return $"투사체 속도 {AppliedValue.Speed.ToString("F1")} 증가";
    }
}
