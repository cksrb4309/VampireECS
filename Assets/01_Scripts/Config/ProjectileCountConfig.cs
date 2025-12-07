using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileCountConfig", menuName = "Config/Projectile/Count")]
public class ProjectileCountConfig : AbilityComponentConfig<ProjectileCountData>
{
    public TierValue<int>[] TierValues;
    public override void ApplyTier(Tier tier)
    {
        TierValue<int> selectValue = TierValues.Where((v) => { return tier == v.Tier; }).First();

        int count = Random.Range(selectValue.MinValue, selectValue.MaxValue);

        BaseComponent.Count = count;
    }
    public override void ApplyStack(ref ProjectileCountData abilityConfig)
    {
        abilityConfig.Count += BaseComponent.Count;
    }
    public override string GetDescription()
    {
        return $"투사체 개수 {BaseComponent.Count.ToString()}개 증가";
    }
}
