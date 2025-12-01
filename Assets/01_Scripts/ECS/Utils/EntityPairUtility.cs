using System.Runtime.CompilerServices;
using Unity.Entities;

public static class EntityPairUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryMatchPair<TA, TB>(
    Entity a,
    Entity b,
    in ComponentLookup<TA> aLookup,
    in ComponentLookup<TB> bLookup,
    out Entity taEntity,
    out Entity tbEntity)
    where TA : unmanaged, IComponentData
    where TB : unmanaged, IComponentData
    {
        if (aLookup.HasComponent(a) && bLookup.HasComponent(b))
        {
            taEntity = a;
            tbEntity = b;

            return true;
        }

        if (aLookup.HasComponent(b) && bLookup.HasComponent(a))
        {
            taEntity = b;
            tbEntity = a;

            return true;
        }

        taEntity = Entity.Null;
        tbEntity = Entity.Null;

        return false;
    }
}
public enum TriggerActionFlags : uint
{
    None = 0,
    DestroyEntityA = 1 << 0,
    DestroyEntityB = 1 << 1,
}