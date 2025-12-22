using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateBefore(typeof(DamageEventSystemGroup))]
public partial class DamageSetupSystemGroup : ComponentSystemGroup { }
