using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

public abstract class AbilityComponentConfig<T> : AbilityConfig
    where T : unmanaged, IComponentData, IAddable<T>
{
    protected T AppliedValue;
    private T baseValue;
    public T BaseValue
    {
        get => baseValue;
        set
        {
            if (baseValue.Equals(value)) return;

            baseValue = value;

            SetValue().Forget();
        }
    }
    private async UniTaskVoid SetValue()
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = em.CreateEntityQuery(typeof(PlayerTag));

        await UniTask.WaitUntil(() =>
        {
            return query.CalculateEntityCount() >= 1;
        });

        Entity player = query.GetSingletonEntity();

        if (em.HasComponent<T>(player)) em.SetComponentData(player, BaseValue);

        else em.AddComponentData(player, BaseValue);
    }


    public override void Apply()
    {
        BaseValue = BaseValue.Add(AppliedValue);
    }
}

public enum Tier
{
    None,
    Bronze,
    Silver,
    Gold,
}

[System.Serializable]
public class TierValue<T> where T : struct
{
    public Tier Tier;
    public T MinValue;
    public T MaxValue;
}