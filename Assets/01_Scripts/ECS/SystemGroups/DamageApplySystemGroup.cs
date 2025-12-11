using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateAfter(typeof(DamageEventSystemGroup))]
public partial class DamageApplySystemGroup : ComponentSystemGroup { }
