using Cysharp.Threading.Tasks;
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class MouseInputBridge : IDisposable
{
    private InputManager inputManager;

    private InputActionReference mouseMoveAction;

    private Camera mainCamera;

    private Entity playerEntity;
    private EntityManager entityManager;

    [Inject]
    public MouseInputBridge(InputManager inputManager)
    {
        this.inputManager = inputManager;
        this.entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Setting().Forget();
    }

    private async UniTaskVoid Setting()
    {
        mainCamera = Camera.main;

        await UniTask.WaitUntil(() =>
        {
            return inputManager.GetInputAction(InputType.MousePosition) != null;
        });

        await UniTask.WaitUntil(() =>
        {
            // 씬에서 Player 태그 가진 엔티티 찾기
            var query = entityManager.CreateEntityQuery(typeof(PlayerTag), typeof(ShooterData));

            if (!query.IsEmpty)
            {
                playerEntity = query.GetSingletonEntity();

                return true;
            }
            else
            {
                return false;
            }
        });

        mouseMoveAction = inputManager.GetInputAction(InputType.MousePosition);

        mouseMoveAction.action.performed += OnMouseMove;
        mouseMoveAction.action.canceled += OnMouseMove;

    }
    public void Dispose()
    {
        mouseMoveAction.action.performed -= OnMouseMove;
        mouseMoveAction.action.canceled -= OnMouseMove;

        inputManager.Release(InputType.MousePosition);
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        if (playerEntity == Entity.Null) return;

        Vector3 mouse = context.ReadValue<Vector2>();

        // NearClipPlane에서 마우스가 가리키는 월드 위치
        Vector3 near = mainCamera.ScreenToWorldPoint(
            new Vector3(mouse.x, mouse.y, mainCamera.nearClipPlane)
        );

        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 aimDir = (near - cameraPos).normalized;

        // Ground y = 0 면과의 교차 계산
        float t = (0f - cameraPos.y) / aimDir.y;
        float3 targetPos = cameraPos + aimDir * t;

        // 플레이어 ShooterData 가져오기
        if (!entityManager.HasComponent<ShooterData>(playerEntity)) return;

        var shooterData = entityManager.GetComponentData<ShooterData>(playerEntity);

        shooterData.Direction = math.normalize(targetPos - entityManager.GetComponentData<LocalToWorld>(playerEntity).Position);

        // 변경된 ShooterData 다시 저장
        entityManager.SetComponentData(playerEntity, shooterData);
    }
}