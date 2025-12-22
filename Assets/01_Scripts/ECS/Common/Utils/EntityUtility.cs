using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public static class EntityUtility
{
    private static readonly Dictionary<ComponentType, Entity> _cachedEntities = new();
    private static readonly Dictionary<ComponentType, object> _cachedComponents = new();

    public static async UniTask<Entity> GetOrWaitForSingletonEntityAsync<T>()
        where T : unmanaged, IComponentData
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        ComponentType key = ComponentType.ReadOnly<T>();

        if (_cachedEntities.TryGetValue(key, out Entity cached))
        {
            if (IsEntityValid<T>(em, cached))
                return cached;

            _cachedEntities.Remove(key);
            _cachedComponents.Remove(key);
        }

        await UniTask.WaitUntil(() =>
        {
            var q = em.CreateEntityQuery(typeof(T));
            return !q.IsEmpty;
        });

        Entity found = em.CreateEntityQuery(typeof(T)).GetSingletonEntity();
        _cachedEntities[key] = found;

        return found;
    }

    public static bool TryGetCachedSingletonEntity<T>(out Entity entity)
        where T : unmanaged, IComponentData
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        ComponentType key = ComponentType.ReadOnly<T>();

        if (_cachedEntities.TryGetValue(key, out entity))
        {
            if (IsEntityValid<T>(em, entity))
                return true;

            _cachedEntities.Remove(key);
            _cachedComponents.Remove(key);
        }

        entity = Entity.Null;
        return false;
    }

    public static async UniTask<T> GetOrWaitForSingletonComponentAsync<T>()
        where T : unmanaged, IComponentData
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        ComponentType key = ComponentType.ReadOnly<T>();

        // 캐시에 있으면 바로 반환
        if (_cachedComponents.TryGetValue(key, out object valueObj) &&
            _cachedEntities.TryGetValue(key, out Entity cachedEnt))
        {
            if (IsEntityValid<T>(em, cachedEnt))
                return (T)valueObj;

            // 캐시 무효화
            _cachedComponents.Remove(key);
            _cachedEntities.Remove(key);
        }

        // 엔티티 기다려서 찾기
        Entity ent = await GetOrWaitForSingletonEntityAsync<T>();

        // 컴포넌트 값 가져오기
        T value = em.GetComponentData<T>(ent);

        // 캐싱
        _cachedComponents[key] = value;

        return value;
    }
    // T가 있다면 가져오기
    public static T GetComponentToSingletone<T>(params ComponentType[] requiredComponents)
        where T : unmanaged, IComponentData
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = em.CreateEntityQuery(requiredComponents);

        int count = query.CalculateEntityCount();

        if (count == 0)
        {
            throw new InvalidOperationException(
                $"싱글톤 컴포넌트 {typeof(T).Name} 이(가) 월드에 존재하지 않습니다.");
        }
        if (count > 1)
        {
            throw new InvalidOperationException(
                $"싱글톤 컴포넌트 {typeof(T).Name} 이(가) 2개 이상 존재합니다. 싱글톤 규칙을 위반합니다.");
        }

        Entity singletonEntity = query.GetSingletonEntity();
        return em.GetComponentData<T>(singletonEntity);
    }
    public static void SetComponentToSingletone<T>(T componentData, params ComponentType[] requiredComponents)
        where T : unmanaged, IComponentData
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = em.CreateEntityQuery(requiredComponents);

        int count = query.CalculateEntityCount();

        if (count == 0)
        {
            throw new InvalidOperationException(
                $"싱글톤 컴포넌트 {typeof(T).Name} 이(가) 월드에 존재하지 않습니다.");
        }
        if (count > 1)
        {
            throw new InvalidOperationException(
                $"싱글톤 컴포넌트 {typeof(T).Name} 이(가) 2개 이상 존재합니다. 싱글톤 규칙을 위반합니다.");
        }

        Entity singletonEntity = query.GetSingletonEntity();
        em.SetComponentData(singletonEntity, componentData);
    }
    public static async UniTask AddOrSetComponentToSingletonAsync<T, U>(U component)
        where T : unmanaged, IComponentData
        where U : unmanaged, IComponentData
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // 싱글톤 T 엔티티 가져오기
        Entity entity = await GetOrWaitForSingletonEntityAsync<T>();

        // Add or Set
        em.AddOrSetComponent(entity, component);
    }
    public static void AddOrSetComponent<T>(this EntityManager em, Entity ent, T component)
        where T : unmanaged, IComponentData
    {
        if (!em.Exists(ent)) return;

        if (em.HasComponent<T>(ent))
            em.SetComponentData(ent, component);
        else
            em.AddComponentData(ent, component);
    }
    public static EntityArchetype CreateArchetype(params ComponentType[] types)
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(types);
    }
    public static Entity CreateEntity()
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
    }
    public static Entity CreateEntity(EntityArchetype archetype)
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype);
    }

    private static bool IsEntityValid<T>(EntityManager em, Entity ent)
        where T : unmanaged, IComponentData
    {
        return em.Exists(ent) && em.HasComponent<T>(ent);
    }
}
