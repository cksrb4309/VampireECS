using Unity.Entities;
using UnityEngine;

public static class PlayerStatApplier<TStats>
    where TStats : unmanaged, IComponentData, IAddable<TStats>
{
    public static async void ApplyStats(TStats stats)
    {
        await EntityUtility.AddOrSetComponentToSingletonAsync<PlayerTag, TStats>(stats);

        Debug.Log("스탯 적용 성공 : " + typeof(TStats).ToString());
    }
}
