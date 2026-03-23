using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateBefore(typeof(DestructionSystemGroup))]
public partial class DestructionCleanupSystemGroup : ComponentSystemGroup { }
