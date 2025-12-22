using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(SpatialCellSetupSystemGroup))]
public partial class SpatialPreUpdateSystemGroup : ComponentSystemGroup { }
