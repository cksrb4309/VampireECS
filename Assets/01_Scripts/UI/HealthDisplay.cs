using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] FloatObservableSO amountSO;
    [SerializeField] FloatObservableSO maxAmountSO;

    private Entity playerEntity = Entity.Null;

    private EntityManager entityManager;

    public void Start()
    {
        Setting().Forget();
    }
    private async UniTaskVoid Setting()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        await UniTask.WaitUntil(() =>
        {
            var query = entityManager.CreateEntityQuery(typeof(PlayerTag));

            return query.CalculateEntityCount() >= 1;
        });

        var query = entityManager.CreateEntityQuery(typeof(PlayerTag));

        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

        playerEntity = entities[0];

        maxAmountSO.Observable.Value = entityManager.GetComponentData<HealthData>(playerEntity).Max;

        entities.Dispose();
    }

    private void LateUpdate()
    {
        if (playerEntity == Entity.Null)
        {
            Debug.Log("playerEntity == Entity.Null");

            return;
        }

        if (!entityManager.Exists(playerEntity))
        {
            Debug.Log("!entityManager.Exists(playerEntity)");

            playerEntity = Entity.Null; return;
        }

        if (!entityManager.HasComponent<HealthData>(playerEntity))
        {
            Debug.Log("!entityManager.HasComponent<HealthData>(playerEntity)");

            return;
        }

        HealthData playerHealth = entityManager.GetComponentData<HealthData>(playerEntity);

        amountSO.Observable.Value = playerHealth.Current;
    }
}
