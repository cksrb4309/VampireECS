using Unity.Entities;

[UpdateInGroup(typeof(CombatRootSystemGroup), OrderLast = true)]
public partial class DestructionSystemGroup : ComponentSystemGroup { }
