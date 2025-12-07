using Unity.Entities;
using UnityEngine;

public abstract class AbilityComponentConfig<T> : AbilityConfig
    where T : struct, IComponentData
{
    [Tooltip("이 설정을 기반으로 ECS 컴포넌트를 초기 생성할 때 사용되는 기본값")]
    public T BaseComponent;
    public T CreateComponent() => BaseComponent;
    public abstract override void ApplyTier(Tier tier);
    public override void ApplyStack(AbilityConfig abilityConfig)
    {
        ApplyStack(abilityConfig as AbilityComponentConfig<T>);
    }
    public abstract void ApplyStack(ref T currentComponent);
    public abstract override string GetDescription();
}

public enum Tier
{
    None,
    Bronze,
    Silver,
    Gold,
}

[System.Serializable]
public class TierValue<T> where T : struct
{
    public Tier Tier;
    public T MinValue;
    public T MaxValue;
}