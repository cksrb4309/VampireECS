using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//public abstract class AbilityConfig : ScriptableObject
//{
//    [Tooltip("능력/효과의 고정 등급")]
//    public Tier SelectTier = Tier.None;

//    [Tooltip("능력/효과의 최대 스택 수")]
//    public int MaxStack = 1;

//    [Tooltip("능력/효과의 아이콘")]
//    public Sprite Icon;

//    [Tooltip("중복 획득 가능 여부")]
//    public bool IsStackable = true;

//    [SerializeField] List<AbilityConfig> prerequisitesAbilities;
//    public List<AbilityConfig> PrerequisitesAbilities => prerequisitesAbilities;
//    public bool IsMaxed(int currentStack) => (currentStack >= MaxStack);
//    public virtual void Initialize() { }
//    public abstract string GetDescription();
//    public abstract void ApplyTier(Tier tier);
//    public abstract void Apply();
//}
public abstract class AbilityConfig : ScriptableObject
{
    public int MaxStack = 1;
    public Sprite Icon;
    public bool IsStackable = true;

    [SerializeField] private List<AbilityConfig> prerequisitesAbilities;

    [HideInInspector] public Tier CurrentTier = Tier.None;

    public IReadOnlyList<AbilityConfig> PrerequisitesAbilities => prerequisitesAbilities;
    public bool IsMaxed(int currentStack) => currentStack >= MaxStack;
    public virtual void Initialize() { }
    public abstract string GetDescription();
    public abstract void ApplyTier(Tier tier);
    public abstract void ApplyToPlayer(
        //EntityCommandBuffer ecb,
        //Entity player
    );
}
public enum Tier
{
    None = -1,
    Bronze = 0,
    Silver = 1,
    Gold = 2,
}

[System.Serializable]
public struct TierValue<T> where T : struct
{
    public Tier Tier;
    public T MinValue;
    public T MaxValue;

    public T GetRandomValue()
    {
        if (typeof(T) == typeof(int))
        {
            int min = (int)(object)MinValue;
            int max = (int)(object)MaxValue;

            int randomValue = Random.Range(min, max + 1);

            return (T)(object)randomValue;
        }
        else if (typeof(T) == typeof(float))
        {
            float min = (float)(object)MinValue;
            float max = (float)(object)MaxValue;

            float randomValue = Random.Range(min, max);

            return (T)(object)randomValue;
        }
        else
        {
            throw new System.InvalidOperationException("Unsupported type for TierValue");
        }
    }
}
