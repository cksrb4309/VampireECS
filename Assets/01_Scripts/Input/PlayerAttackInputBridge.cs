using Cysharp.Threading.Tasks;
using System;
using Unity.Entities;
using UnityEngine.InputSystem;
using VContainer;
public class PlayerAttackInputBridge : IDisposable
{
    private readonly InputManager inputManager;
    private readonly EntityManager entityManager;

    private InputActionReference attackAction;

    private Entity playerEntity = Entity.Null;

    private EntityQuery playerQuery;

    [Inject]
    public PlayerAttackInputBridge(InputManager inputManager)
    {
        this.inputManager = inputManager;
        this.entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        playerQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadOnly<ShooterData>()
        );

        Setting().Forget();
    }

    private async UniTaskVoid Setting()
    {
        // Input 준비될 때까지 대기
        await UniTask.WaitUntil(() =>
            inputManager.GetInputAction(InputType.PlayerAttack) != null);

        // Player 엔티티 대기
        await UniTask.WaitUntil(TryResolvePlayer);

        // 초기 상태: 발사 불가
        SetCanFire(false);

        attackAction = inputManager.GetInputAction(InputType.PlayerAttack);

        attackAction.action.started += OnAttackStarted;
        attackAction.action.canceled += OnAttackCanceled;
    }

    private bool TryResolvePlayer()
    {
        if (playerQuery.IsEmpty) return false;

        playerEntity = playerQuery.GetSingletonEntity();
        return true;
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        SetCanFire(true);
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        SetCanFire(false);
    }

    private void SetCanFire(bool canFire)
    {
        if (playerEntity == Entity.Null)
            return;

        if (!entityManager.Exists(playerEntity))
        {
            playerEntity = Entity.Null;
            return;
        }

        if (!entityManager.HasComponent<ShooterCanFireData>(playerEntity))
            return;

        entityManager.SetComponentData(playerEntity, new ShooterCanFireData
        {
            CanFire = canFire
        });
    }

    public void Dispose()
    {
        if (attackAction != null)
        {
            attackAction.action.started -= OnAttackStarted;
            attackAction.action.canceled -= OnAttackCanceled;
        }

        inputManager.Release(InputType.PlayerAttack);
    }
}
