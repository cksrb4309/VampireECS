using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateAfter(typeof(DamageApplySystemGroup))]
public partial class DeathSystemGroup : ComponentSystemGroup { }
