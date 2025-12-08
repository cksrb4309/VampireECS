using Unity.Entities;
using UnityEngine;

public class LevelUpUIBridge : MonoBehaviour
{
    AbilityRewardController abilityRewardController;
    private void Start()
    {
        abilityRewardController = FindFirstObjectByType<AbilityRewardController>();
    }
    void Update()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = em.CreateEntityQuery(typeof(LevelUpUIRequest));

        if (query.CalculateEntityCount() > 0)
        {
            // 이벤트 삭제
            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var e in entities)
                em.DestroyEntity(e);

            Debug.Log("레벨업 카운팅 횟수 : " + entities.Length);
            abilityRewardController.GenerateRewardChoices();
        }
    }
}
