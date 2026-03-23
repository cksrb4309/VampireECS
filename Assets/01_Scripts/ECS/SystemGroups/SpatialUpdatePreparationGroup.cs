using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(SpatialSetupSystemGroup))]
public partial class SpatialUpdatePreparationGroup : ComponentSystemGroup { }
