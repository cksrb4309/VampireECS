using UnityEngine;
using Unity.Entities;
public class TimePauseController : MonoBehaviour
{
    public void Awake()
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        em.AddComponentData(em.CreateEntity(), new GameTimeScale { Value = 1f });
    }
    public void SetPause(bool isPaused)
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = em.CreateEntityQuery(typeof(GameTimeScale));

        if (query.CalculateEntityCount() <= 0) return;

        var e = query.GetSingletonEntity();

        em.SetComponentData(e, new GameTimeScale
        {
            Value = isPaused ? 0f : 1f
        });

        Time.timeScale = isPaused ? 0f : 1f;
    }
}
