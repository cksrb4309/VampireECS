using UnityEngine;
using Unity.Entities;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Transforms;

public class FollowCamera : MonoBehaviour
{
    public Vector3 offset;

    private Entity target = Entity.Null;

    private EntityManager entityManager;

    public void Start()
    {
        Setting().Forget();
    }

    private async UniTaskVoid Setting()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        while (target == Entity.Null)
        {
            var query = entityManager.CreateEntityQuery(typeof(PlayerTag));

            await UniTask.WaitUntil(() =>
            {
                query = entityManager.CreateEntityQuery(typeof(PlayerTag));

                return query.CalculateEntityCount() >= 1;
            });
            NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

            target = entities[0];

            entities.Dispose();
        }
    }

    private void LateUpdate()
    {
        if (target == Entity.Null)
            return;

        if (!entityManager.Exists(target))
        {
            target = Entity.Null;
            return;
        }

        if (!entityManager.HasComponent<LocalToWorld>(target))
            return;

        var playerLTW = entityManager.GetComponentData<LocalToWorld>(target);
        Vector3 playerPosition = playerLTW.Position;

        transform.position = playerPosition + offset;
    }
}
