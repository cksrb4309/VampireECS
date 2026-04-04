using System;
using System.Collections.Generic;
using System.IO;
using Unity.Entities;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Transforms;

public static class BatchValidationRunner
{
    private const string TestCombatScenePath = "Assets/07_Scenes/Test_Combat.unity";
    private const string CombatSettingPrefabPath = "Assets/06_Prefabs/Scene/CombatSetting.prefab";

    private static readonly string[] RequiredAssetPaths =
    {
        TestCombatScenePath,
        CombatSettingPrefabPath,
        "Assets/00_Core/ProjectSetting/InputSystem_Actions.inputactions",
        "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats/AuraStatsConfig.asset",
        "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats/BoomerangStatsConfig.asset",
        "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats/CombatStatsConfig.asset",
        "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats/ShooterStatsConfig.asset",
        "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock/UnlockAuraConfig.asset",
        "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock/UnlockBoomerangConfig.asset",
        "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock/UnlockShooterConfig.asset",
    };

    public static void RunProjectSmokeValidation()
    {
        RunProjectSmokeValidationCore(requireEnabledBuildScene: false);
    }

    public static void RunProjectSmokeValidationStrict()
    {
        RunProjectSmokeValidationCore(requireEnabledBuildScene: true);
    }

    public static void RunProjectEditModeSmokeTests()
    {
        RunApplyDamageSystemLethalDamageScenario();
        RunApplyDamageSystemMissingTargetScenario();

        Debug.Log("[BatchValidation] EditMode smoke tests passed.");
    }

    [MenuItem("Tools/Codex Validation/Run Smoke Validation", priority = 1800)]
    public static void RunSmokeValidationFromMenu()
    {
        RunMenuValidation("Smoke Validation", RunProjectSmokeValidation);
    }

    [MenuItem("Tools/Codex Validation/Run Strict Smoke Validation", priority = 1801)]
    public static void RunStrictSmokeValidationFromMenu()
    {
        RunMenuValidation("Strict Smoke Validation", RunProjectSmokeValidationStrict);
    }

    [MenuItem("Tools/Codex Validation/Run EditMode Smoke Tests", priority = 1802)]
    public static void RunEditModeSmokeTestsFromMenu()
    {
        RunMenuValidation("EditMode Smoke Tests", RunProjectEditModeSmokeTests);
    }

    [MenuItem("Tools/Codex Validation/Run Full Validation", priority = 1803)]
    public static void RunFullValidationFromMenu()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Codex Validation", "Running strict smoke validation...", 0.33f);
            RunProjectSmokeValidationStrict();

            EditorUtility.DisplayProgressBar("Codex Validation", "Running EditMode smoke tests...", 0.9f);
            RunProjectEditModeSmokeTests();

            Debug.Log("[CodexValidationMenu] Full validation passed.");
            EditorUtility.DisplayDialog("Codex Validation", "Full validation passed.", "OK");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Codex Validation", $"Full validation failed.\n\n{exception.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    [MenuItem("Tools/Codex Validation/Open Validation Log Folder", priority = 1820)]
    public static void OpenValidationLogFolder()
    {
        string logFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Logs", "validation"));
        Directory.CreateDirectory(logFolder);
        EditorUtility.RevealInFinder(logFolder);
    }

    private static void RunProjectSmokeValidationCore(bool requireEnabledBuildScene)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        foreach (string assetPath in RequiredAssetPaths)
            ValidateRequiredAsset(assetPath, errors);

        ValidateSceneForMissingScripts(TestCombatScenePath, errors);
        ValidatePrefabForMissingScripts(CombatSettingPrefabPath, errors);
        ValidateBuildSettings(TestCombatScenePath, requireEnabledBuildScene, errors, warnings);
        ValidateTestCoverage(warnings);

        foreach (string warning in warnings)
            Debug.LogWarning($"[BatchValidation] {warning}");

        if (errors.Count > 0)
        {
            foreach (string error in errors)
                Debug.LogError($"[BatchValidation] {error}");

            throw new BuildFailedException($"Project smoke validation failed with {errors.Count} error(s).");
        }

        Debug.Log($"[BatchValidation] Smoke validation passed with {warnings.Count} warning(s).");
    }

    private static void ValidateRequiredAsset(string assetPath, List<string> errors)
    {
        if (AssetDatabase.LoadMainAssetAtPath(assetPath) == null)
            errors.Add($"Missing required asset: {assetPath}");
    }

    private static void ValidateSceneForMissingScripts(string scenePath, List<string> errors)
    {
        if (AssetDatabase.LoadMainAssetAtPath(scenePath) == null)
            return;

        Scene scene = default;
        bool opened = false;

        try
        {
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            opened = true;

            int missingScriptCount = 0;
            foreach (GameObject root in scene.GetRootGameObjects())
                missingScriptCount += CountMissingScripts(root);

            if (missingScriptCount > 0)
                errors.Add($"{scenePath} contains {missingScriptCount} missing MonoBehaviour reference(s).");
        }
        catch (Exception exception)
        {
            errors.Add($"{scenePath} could not be opened for validation: {exception.Message}");
        }
        finally
        {
            if (opened && scene.IsValid())
                EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static void ValidatePrefabForMissingScripts(string prefabPath, List<string> errors)
    {
        if (AssetDatabase.LoadMainAssetAtPath(prefabPath) == null)
            return;

        GameObject prefabRoot = null;

        try
        {
            prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            int missingScriptCount = CountMissingScripts(prefabRoot);

            if (missingScriptCount > 0)
                errors.Add($"{prefabPath} contains {missingScriptCount} missing MonoBehaviour reference(s).");
        }
        catch (Exception exception)
        {
            errors.Add($"{prefabPath} could not be opened for validation: {exception.Message}");
        }
        finally
        {
            if (prefabRoot != null)
                PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private static void ValidateBuildSettings(
        string scenePath,
        bool requireEnabledBuildScene,
        List<string> errors,
        List<string> warnings)
    {
        foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
        {
            if (buildScene.path == scenePath && buildScene.enabled)
                return;
        }

        string message = $"Scene is not enabled in ProjectSettings/EditorBuildSettings.asset: {scenePath}";

        if (requireEnabledBuildScene)
            errors.Add(message);
        else
            warnings.Add(message);
    }

    private static void ValidateTestCoverage(List<string> warnings)
    {
        string[] scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/99_Tests" });
        if (scripts.Length == 0)
            warnings.Add("Assets/99_Tests does not contain project test scripts yet.");
    }

    private static int CountMissingScripts(GameObject gameObject)
    {
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);

        foreach (Transform child in gameObject.transform)
            count += CountMissingScripts(child.gameObject);

        return count;
    }

    private static void RunMenuValidation(string title, Action action)
    {
        try
        {
            EditorUtility.DisplayProgressBar("Codex Validation", $"{title} running...", 0.5f);
            action();

            Debug.Log($"[CodexValidationMenu] {title} passed.");
            EditorUtility.DisplayDialog("Codex Validation", $"{title} passed.", "OK");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Codex Validation", $"{title} failed.\n\n{exception.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void RunApplyDamageSystemLethalDamageScenario()
    {
        using var world = new World("BatchValidation.ApplyDamageSystem.Lethal");
        EntityManager entityManager = world.EntityManager;

        var group = world.CreateSystemManaged<DamageApplySystemGroup>();
        SystemHandle applyDamageSystem = world.CreateSystem<ApplyDamageSystem>();
        group.AddSystemToUpdateList(applyDamageSystem);
        group.SortSystems();

        Entity target = entityManager.CreateEntity(typeof(HealthData), typeof(LocalTransform));
        entityManager.SetComponentData(target, new HealthData
        {
            Current = 10f,
            Max = 10f
        });
        entityManager.SetComponentData(target, LocalTransform.FromPosition(new Unity.Mathematics.float3(3f, 0f, 2f)));

        Entity damageEventEntity = entityManager.CreateEntity(typeof(DamageEventData));
        entityManager.SetComponentData(damageEventEntity, new DamageEventData
        {
            Target = target,
            Damage = 15f
        });

        group.Update();

        HealthData updatedHealth = entityManager.GetComponentData<HealthData>(target);
        if (updatedHealth.Current != -5f)
            throw new BuildFailedException($"ApplyDamageSystem lethal scenario failed. Expected health -5 but got {updatedHealth.Current}.");

        if (!entityManager.HasComponent<DeadTag>(target))
            throw new BuildFailedException("ApplyDamageSystem lethal scenario failed. DeadTag was not added.");

        if (entityManager.Exists(damageEventEntity))
            throw new BuildFailedException("ApplyDamageSystem lethal scenario failed. Damage event entity was not consumed.");

        using EntityQuery damageTextQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DamageTextEvent>());
        if (damageTextQuery.CalculateEntityCount() != 1)
            throw new BuildFailedException("ApplyDamageSystem lethal scenario failed. DamageTextEvent was not emitted exactly once.");
    }

    private static void RunApplyDamageSystemMissingTargetScenario()
    {
        using var world = new World("BatchValidation.ApplyDamageSystem.MissingTarget");
        EntityManager entityManager = world.EntityManager;

        var group = world.CreateSystemManaged<DamageApplySystemGroup>();
        SystemHandle applyDamageSystem = world.CreateSystem<ApplyDamageSystem>();
        group.AddSystemToUpdateList(applyDamageSystem);
        group.SortSystems();

        Entity missingTarget = entityManager.CreateEntity(typeof(HealthData));
        entityManager.DestroyEntity(missingTarget);

        Entity damageEventEntity = entityManager.CreateEntity(typeof(DamageEventData));
        entityManager.SetComponentData(damageEventEntity, new DamageEventData
        {
            Target = missingTarget,
            Damage = 5f
        });

        group.Update();

        if (entityManager.Exists(damageEventEntity))
            throw new BuildFailedException("ApplyDamageSystem missing target scenario failed. Damage event entity was not consumed.");

        using EntityQuery damageTextQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DamageTextEvent>());
        if (damageTextQuery.CalculateEntityCount() != 0)
            throw new BuildFailedException("ApplyDamageSystem missing target scenario failed. DamageTextEvent should not have been emitted.");
    }
}
