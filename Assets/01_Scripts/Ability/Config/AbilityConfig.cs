using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityConfig : ScriptableObject
{
    [Tooltip("능력/효과의 고정 등급")]
    public Tier SelectTier = Tier.None;

    [Tooltip("능력/효과의 최대 스택 수")]
    public int MaxStack = 1;

    [Tooltip("능력/효과의 아이콘")]
    public Sprite Icon;

    [Tooltip("중복 획득 가능 여부")]
    public bool IsStackable = true;

    [SerializeField] List<AbilityConfig> prerequisitesAbilities;
    public List<AbilityConfig> PrerequisitesAbilities => prerequisitesAbilities;
    public bool IsMaxed(int currentStack) => (currentStack >= MaxStack);
    public virtual void Initialize() { }
    public abstract string GetDescription();
    public abstract void ApplyTier(Tier tier);
    public abstract void Apply();
}
