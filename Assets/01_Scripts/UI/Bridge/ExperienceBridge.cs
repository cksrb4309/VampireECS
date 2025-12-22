using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

public class ExperienceBridge : MonoBehaviour
{
    [SerializeField] IntObservableSO amountSO;
    [SerializeField] IntObservableSO requiredSO;
    [SerializeField] IntObservableSO levelSO;

    private Entity playerEntity = Entity.Null;
    private EntityManager em;

    private AbilityRewardGenerator rewardUI;
    private TimePauseController timePauseController;

    private async void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        rewardUI = FindFirstObjectByType<AbilityRewardGenerator>();

        timePauseController = FindFirstObjectByType<TimePauseController>();

        await WaitPlayerReady();
    }

    private async UniTask WaitPlayerReady()
    {
        await UniTask.WaitUntil(() =>
        {
            var query = em.CreateEntityQuery(typeof(PlayerTag), typeof(PlayerExpData));
            if (query.IsEmpty) return false;

            playerEntity = query.GetSingletonEntity();
            return true;
        });
    }

    private void LateUpdate()
    {
        if (playerEntity == Entity.Null) return;
        if (!em.Exists(playerEntity)) return;

        // PlayerExpData 값을 UI로 전달
        var exp = em.GetComponentData<PlayerExpData>(playerEntity);

        if (amountSO != null)
            amountSO.Observable.Value = exp.Current;

        if (requiredSO != null)
            requiredSO.Observable.Value = exp.Required;

        if (levelSO != null)
            levelSO.Observable.Value = exp.Level;

        // 레벨업 요청 감지
        var query = em.CreateEntityQuery(typeof(LevelUpUIRequest));

        if (query.CalculateEntityCount() > 0)
        {
            timePauseController.SetPause(true);

            // 이벤트 삭제
            var ents = query.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var e in ents)
            {
                em.DestroyEntity(e);
            }

            rewardUI.GenerateRewardChoices();
        }
    }
}
