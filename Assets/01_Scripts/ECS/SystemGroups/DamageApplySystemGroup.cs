using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateBefore(typeof(DestructionCleanupSystemGroup))]
public partial class DamageApplySystemGroup : ComponentSystemGroup { }
