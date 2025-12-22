using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateBefore(typeof(DamageApplySystemGroup))]
public partial class DamageEventSystemGroup : ComponentSystemGroup { }
