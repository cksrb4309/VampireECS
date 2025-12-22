using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateBefore(typeof(PreDestructionCleanupSystemGroup))]
public partial class DamageApplySystemGroup : ComponentSystemGroup { }
