using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup))]
[UpdateBefore(typeof(DamageSetupSystemGroup))]
public partial class SpatialCellSetupSystemGroup : ComponentSystemGroup { }
