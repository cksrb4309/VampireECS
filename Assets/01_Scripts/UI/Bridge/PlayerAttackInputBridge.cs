using Cysharp.Threading.Tasks;
using System;
using UnityEngine.InputSystem;
using VContainer;
public class PlayerAttackInputBridge : IDisposable
{
    private readonly InputManager inputManager;

    private InputActionReference attackAction;

    [Inject]
    public PlayerAttackInputBridge(InputManager inputManager)
    {
        this.inputManager = inputManager;

        Setting().Forget();
    }
    private async UniTaskVoid Setting()
    {
        // Input 준비될 때까지 대기
        await UniTask.WaitUntil(() =>
            inputManager.GetInputAction(InputType.PlayerAttack) != null);

        attackAction = inputManager.GetInputAction(InputType.PlayerAttack);

        attackAction.action.started += OnAttackStarted;
        attackAction.action.canceled += OnAttackCanceled;
    }
    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        AttackInputEventBus.Publish(new AttackIsPressedMessage { IsPressed = true });
    }
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        AttackInputEventBus.Publish(new AttackIsPressedMessage { IsPressed = false });
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
public static class AttackInputEventBus
{
    public static IDisposable Subscribe(Action<AttackIsPressedMessage> handler)
        => EventBus<AttackIsPressedMessage>.Subscribe(handler);

    public static void SubscribeOnce(Action<AttackIsPressedMessage> handler)
        => EventBus<AttackIsPressedMessage>.SubscribeOnce(handler);

    public static void Unsubscribe(Action<AttackIsPressedMessage> handler)
        => EventBus<AttackIsPressedMessage>.Unsubscribe(handler);

    public static void Publish(AttackIsPressedMessage message)
        => EventBus<AttackIsPressedMessage>.Publish(message);

    public static void UnsubscribeAll()
        => EventBus<AttackIsPressedMessage>.UnsubscribeAll();
}
public struct AttackIsPressedMessage
{
    public bool IsPressed;
}
