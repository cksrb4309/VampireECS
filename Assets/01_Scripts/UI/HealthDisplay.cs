using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text textLabel;
    [SerializeField] Image fillImage;

    private Entity playerEntity = Entity.Null;

    private EntityManager entityManager;

    private float beforeHealth = 0;
    private float maxHealth = 0;

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

        maxHealth = entityManager.GetComponentData<HealthData>(playerEntity).Max;

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

        SetValue(playerHealth.Current);
    }
    private void SetValue(float value)
    {
        if (beforeHealth != value)
        {
            beforeHealth = value;

            textLabel.text = value.ToString();

            fillImage.fillAmount = value != 0f ? value / maxHealth : 0f;
        }
    }
}
