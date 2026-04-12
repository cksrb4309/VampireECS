using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ApplyDamageSystemEditModeTests
{
    private static readonly MethodInfo SetComponentDataMethod = typeof(EntityManager)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(method =>
            method.Name == nameof(EntityManager.SetComponentData) &&
            method.IsGenericMethodDefinition &&
            method.GetParameters().Length == 2 &&
            method.GetParameters()[0].ParameterType == typeof(Entity));

    private static readonly MethodInfo GetComponentDataMethod = typeof(EntityManager)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(method =>
            method.Name == nameof(EntityManager.GetComponentData) &&
            method.IsGenericMethodDefinition &&
            method.GetParameters().Length == 1 &&
            method.GetParameters()[0].ParameterType == typeof(Entity));

    private World world;
    private EntityManager entityManager;
    private ComponentSystemGroup damageApplyGroup;

    private Type applyDamageSystemType;
    private Type damageApplySystemGroupType;
    private Type healthDataType;
    private Type damageEventDataType;
    private Type deadTagType;
    private Type damageTextEventType;

    [SetUp]
    public void SetUp()
    {
        applyDamageSystemType = RequireGameType("ApplyDamageSystem");
        damageApplySystemGroupType = RequireGameType("DamageApplySystemGroup");
        healthDataType = RequireGameType("HealthData");
        damageEventDataType = RequireGameType("DamageEventData");
        deadTagType = RequireGameType("DeadTag");
        damageTextEventType = RequireGameType("DamageTextEvent");

        world = new World(nameof(ApplyDamageSystemEditModeTests));
        entityManager = world.EntityManager;
        damageApplyGroup = (ComponentSystemGroup)world.CreateSystemManaged(damageApplySystemGroupType);

        SystemHandle applyDamageSystem = world.CreateSystem(applyDamageSystemType);
        damageApplyGroup.AddSystemToUpdateList(applyDamageSystem);
        damageApplyGroup.SortSystems();
    }

    [TearDown]
    public void TearDown()
    {
        if (world is { IsCreated: true })
            world.Dispose();
    }

    [Test]
    public void LethalDamage_AddsDeadTag_And_EmitsDamageTextEvent()
    {
        Entity target = entityManager.CreateEntity(
            ComponentType.ReadWrite(healthDataType),
            ComponentType.ReadWrite<LocalTransform>());

        SetComponentDataBoxed(target, healthDataType, CreateStructBox(
            healthDataType,
            ("Current", 10f),
            ("Max", 10f)));

        entityManager.SetComponentData(target, LocalTransform.FromPosition(new float3(3f, 0f, 2f)));

        Entity damageEventEntity = entityManager.CreateEntity(ComponentType.ReadWrite(damageEventDataType));
        SetComponentDataBoxed(damageEventEntity, damageEventDataType, CreateStructBox(
            damageEventDataType,
            ("Target", target),
            ("Damage", 15f)));

        damageApplyGroup.Update();

        object updatedHealth = GetComponentDataBoxed(target, healthDataType);
        Assert.That((float)GetFieldValue(updatedHealth, "Current"), Is.EqualTo(-5f));
        Assert.That(entityManager.HasComponent(target, ComponentType.ReadWrite(deadTagType)), Is.True);
        Assert.That(entityManager.Exists(damageEventEntity), Is.False);

        using EntityQuery damageTextQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly(damageTextEventType));
        Assert.That(damageTextQuery.CalculateEntityCount(), Is.EqualTo(1));

        Entity damageTextEntity = damageTextQuery.GetSingletonEntity();
        object damageTextEvent = GetComponentDataBoxed(damageTextEntity, damageTextEventType);
        float3 worldPosition = (float3)GetFieldValue(damageTextEvent, "WorldPosition");

        Assert.That((int)GetFieldValue(damageTextEvent, "Damage"), Is.EqualTo(15));
        Assert.That(worldPosition.x, Is.EqualTo(3f));
        Assert.That(worldPosition.y, Is.EqualTo(0f));
        Assert.That(worldPosition.z, Is.EqualTo(2f));
    }

    [Test]
    public void MissingTarget_ConsumesEvent_WithoutEmittingDamageText()
    {
        Entity target = entityManager.CreateEntity(ComponentType.ReadWrite(healthDataType));
        entityManager.DestroyEntity(target);

        Entity damageEventEntity = entityManager.CreateEntity(ComponentType.ReadWrite(damageEventDataType));
        SetComponentDataBoxed(damageEventEntity, damageEventDataType, CreateStructBox(
            damageEventDataType,
            ("Target", target),
            ("Damage", 5f)));

        damageApplyGroup.Update();

        Assert.That(entityManager.Exists(damageEventEntity), Is.False);

        using EntityQuery damageTextQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly(damageTextEventType));
        Assert.That(damageTextQuery.CalculateEntityCount(), Is.EqualTo(0));
    }

    private static Type RequireGameType(string typeName)
    {
        return Type.GetType($"{typeName}, Assembly-CSharp", throwOnError: true);
    }

    private static object CreateStructBox(Type type, params (string Name, object Value)[] fields)
    {
        object boxedValue = Activator.CreateInstance(type);
        foreach ((string name, object value) in fields)
            type.GetField(name).SetValue(boxedValue, value);

        return boxedValue;
    }

    private void SetComponentDataBoxed(Entity entity, Type componentType, object value)
    {
        SetComponentDataMethod
            .MakeGenericMethod(componentType)
            .Invoke(entityManager, new[] { (object)entity, value });
    }

    private object GetComponentDataBoxed(Entity entity, Type componentType)
    {
        return GetComponentDataMethod
            .MakeGenericMethod(componentType)
            .Invoke(entityManager, new object[] { entity });
    }

    private static object GetFieldValue(object boxedValue, string fieldName)
    {
        return boxedValue.GetType().GetField(fieldName).GetValue(boxedValue);
    }
}
