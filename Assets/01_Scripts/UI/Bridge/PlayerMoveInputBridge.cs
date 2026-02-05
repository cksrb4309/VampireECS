using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using VContainer;
using Cysharp.Threading.Tasks;
using Unity.Collections;
public class PlayerMoveInputBridge : IDisposable
{
    private InputManager inputManager;
    private InputActionReference input;

    private Entity[] multiplePlayers; // 여러 플레이어 저장

    private Entity playerEntity;
    private EntityManager entityManager;

    [Inject]
    public PlayerMoveInputBridge(InputManager inputManager)
    {
        this.inputManager = inputManager;

        //Setting().Forget();
        Setting_2().Forget();
    }
    private async UniTaskVoid Setting()
    {
        // EntityManager 초기화
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = new EntityQuery();

        await UniTask.WaitUntil(() =>
        {
            query = entityManager.CreateEntityQuery(typeof(PlayerTag));

            return query.CalculateEntityCount() >= 1;
        });

        playerEntity = query.GetSingletonEntity();

        //Enable
        input = inputManager.GetInputAction(InputType.PlayerMove);

        input.action.performed += OnMove;
        input.action.canceled += OnMove;
    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (!entityManager.Exists(playerEntity))
        {
            // Disable

            return;
        }

        Vector2 v = ctx.ReadValue<Vector2>();
        entityManager.SetComponentData(playerEntity, new PlayerInputData { Move = new float2(v.x, v.y) });
    }
    public void Dispose()
    {
        input.action.performed -= OnMove;
        input.action.canceled -= OnMove;

        inputManager.Release(InputType.PlayerMove);
    }
    private async UniTaskVoid Setting_2()
    {
        // EntityManager 초기화
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // PlayerTag 가진 엔티티가 여러 개 생성될 때까지 기다리기
        await UniTask.WaitUntil(() =>
        {
            var query = entityManager.CreateEntityQuery(typeof(PlayerTag));

            int count = query.CalculateEntityCount();

            return count >= 1;
        });

        EntityQuery query = entityManager.CreateEntityQuery(typeof(PlayerTag));
        NativeArray<Entity> players = default;
        players = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        // 여러 플레이어 엔티티 저장
        multiplePlayers = players.ToArray();
        players.Dispose();

        // 입력 바인딩
        input = inputManager.GetInputAction(InputType.PlayerMove);

        input.action.performed += OnMove_Multi;
        input.action.canceled += OnMove_Multi;
    }


    private void OnMove_Multi(InputAction.CallbackContext ctx)
    {
        if (multiplePlayers == null || multiplePlayers.Length == 0)
            return;

        Vector2 v = ctx.ReadValue<Vector2>();
        float2 move = new float2(v.x, v.y);

        // 전체 플레이어 엔티티에 입력 적용
        foreach (var entity in multiplePlayers)
        {
            if (!entityManager.Exists(entity))  continue;

            entityManager.SetComponentData(entity, new PlayerInputData { Move = move });
        }
    }
}
