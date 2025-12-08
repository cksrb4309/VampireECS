using Unity.Entities;
using UnityEngine;

public abstract class AbilityComponentConfig<T> : AbilityConfig
    where T : struct, IComponentData
{
    [Tooltip("게임 진행 중 값을 저장")]
    public T BaseComponent;

    [Tooltip("적용할 값을 저장")]
    public T ApplyComponent;
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