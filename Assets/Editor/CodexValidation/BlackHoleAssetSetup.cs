using System;
using UnityEditor;
using UnityEngine;

public static class BlackHoleAssetSetup
{
    private const string StatsFolderPath = "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats";
    private const string UnlockFolderPath = "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock";
    private const string StatsAssetPath = StatsFolderPath + "/BlackHoleStatsConfig.asset";
    private const string UnlockAssetPath = UnlockFolderPath + "/UnlockBlackHoleConfig.asset";
    private const string CombatSettingPrefabPath = "Assets/06_Prefabs/Scene/CombatSetting.prefab";

    [MenuItem("Tools/Codex Validation/Setup Black Hole Assets")]
    public static void SetupBlackHoleAssetsMenu()
    {
        SetupBlackHoleAssets(logToConsole: true);
    }

    public static void SetupBlackHoleAssetsBatch()
    {
        SetupBlackHoleAssets(logToConsole: true);
    }

    private static void SetupBlackHoleAssets(bool logToConsole)
    {
        EnsureFolderExists(StatsFolderPath);
        EnsureFolderExists(UnlockFolderPath);

        BlackHoleStatsConfig statsConfig = LoadOrCreateAsset<BlackHoleStatsConfig>(StatsAssetPath);
        UnlockBlackHoleConfig unlockConfig = LoadOrCreateAsset<UnlockBlackHoleConfig>(UnlockAssetPath);

        ConfigureBlackHoleStats(statsConfig);
        ConfigureUnlockBlackHole(unlockConfig, statsConfig);
        ConnectCombatSettingPrefab(unlockConfig);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (logToConsole)
        {
            Debug.Log($"[CodexSetup] Black hole assets configured. Stats='{StatsAssetPath}', Unlock='{UnlockAssetPath}', Prefab='{CombatSettingPrefabPath}'.");
        }
    }

    private static T LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
    {
        T existingAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (existingAsset != null)
        {
            return existingAsset;
        }

        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, assetPath);
        return asset;
    }

    private static void ConfigureBlackHoleStats(BlackHoleStatsConfig config)
    {
        SerializedObject serializedObject = new SerializedObject(config);

        serializedObject.FindProperty("m_Name").stringValue = "BlackHoleStatsConfig";
        serializedObject.FindProperty("MaxStack").intValue = 200;
        serializedObject.FindProperty("IsStackable").boolValue = true;
        serializedObject.FindProperty("Icon").objectReferenceValue = null;
        serializedObject.FindProperty("CurrentTier").intValue = (int)Tier.None;

        SetObjectArray(serializedObject.FindProperty("prerequisitesAbilities"), Array.Empty<AbilityConfig>());

        SetTierFloatArray(serializedObject.FindProperty("damageValue"),
            (Tier.Bronze, 0.125f, 0.25f),
            (Tier.Silver, 0.25f, 0.5f),
            (Tier.Gold, 0.5f, 0.875f));

        SetTierFloatArray(serializedObject.FindProperty("attackSpeedValue"),
            (Tier.Bronze, 0.14285715f, 0.2857143f),
            (Tier.Silver, 0.2857143f, 0.5714286f),
            (Tier.Gold, 0.5714286f, 0.85714287f));

        SetTierFloatArray(serializedObject.FindProperty("acquireRadiusValue"),
            (Tier.Bronze, 1f, 2f),
            (Tier.Silver, 2f, 3f),
            (Tier.Gold, 3f, 5f));

        SetTierFloatArray(serializedObject.FindProperty("radiusValue"),
            (Tier.Bronze, 0.25f, 0.5f),
            (Tier.Silver, 0.5f, 0.75f),
            (Tier.Gold, 0.75f, 1.25f));

        SetTierFloatArray(serializedObject.FindProperty("durationValue"),
            (Tier.Bronze, 0.25f, 0.5f),
            (Tier.Silver, 0.5f, 0.75f),
            (Tier.Gold, 0.75f, 1f));

        SetTierFloatArray(serializedObject.FindProperty("tickIntervalValue"),
            (Tier.Bronze, -0.02f, -0.01f),
            (Tier.Silver, -0.04f, -0.02f),
            (Tier.Gold, -0.08f, -0.04f));

        SetTierFloatArray(serializedObject.FindProperty("pullStrengthValue"),
            (Tier.Bronze, 0.5f, 1f),
            (Tier.Silver, 1f, 2f),
            (Tier.Gold, 2f, 3f));

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(config);
    }

    private static void ConfigureUnlockBlackHole(UnlockBlackHoleConfig config, BlackHoleStatsConfig prerequisiteStats)
    {
        SerializedObject serializedObject = new SerializedObject(config);

        serializedObject.FindProperty("m_Name").stringValue = "UnlockBlackHoleConfig";
        serializedObject.FindProperty("MaxStack").intValue = 1;
        serializedObject.FindProperty("IsStackable").boolValue = false;
        serializedObject.FindProperty("Icon").objectReferenceValue = null;
        serializedObject.FindProperty("CurrentTier").intValue = (int)Tier.None;
        serializedObject.FindProperty("fixedTier").intValue = (int)Tier.Gold;
        serializedObject.FindProperty("description").stringValue = "가장 가까운 적 위치에 흡인하는 블랙홀을 생성합니다";
        serializedObject.FindProperty("baseDamage").floatValue = 4f;
        serializedObject.FindProperty("baseAttackSpeed").floatValue = 0.35f;
        serializedObject.FindProperty("baseAcquireRadius").floatValue = 10f;
        serializedObject.FindProperty("baseRadius").floatValue = 4f;
        serializedObject.FindProperty("baseDuration").floatValue = 2.5f;
        serializedObject.FindProperty("baseTickInterval").floatValue = 0.25f;
        serializedObject.FindProperty("basePullStrength").floatValue = 7.5f;

        SetObjectArray(
            serializedObject.FindProperty("prerequisitesAbilities"),
            new AbilityConfig[] { prerequisiteStats });

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(config);
    }

    private static void ConnectCombatSettingPrefab(UnlockBlackHoleConfig unlockConfig)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(CombatSettingPrefabPath);

        try
        {
            AbilityConfigInitializer initializer = prefabRoot.GetComponentInChildren<AbilityConfigInitializer>(true);
            AbilityRewardGenerator rewardGenerator = prefabRoot.GetComponentInChildren<AbilityRewardGenerator>(true);

            if (initializer == null)
            {
                throw new InvalidOperationException($"AbilityConfigInitializer not found in '{CombatSettingPrefabPath}'.");
            }

            if (rewardGenerator == null)
            {
                throw new InvalidOperationException($"AbilityRewardGenerator not found in '{CombatSettingPrefabPath}'.");
            }

            SerializedObject initializerObject = new SerializedObject(initializer);
            SerializedProperty abilitiesProperty = initializerObject.FindProperty("abilities");
            AddObjectReferenceIfMissing(abilitiesProperty, unlockConfig);
            initializerObject.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject rewardGeneratorObject = new SerializedObject(rewardGenerator);
            SerializedProperty rewardCandidatesProperty = rewardGeneratorObject.FindProperty("rewardCandidates");
            AddObjectReferenceIfMissing(rewardCandidatesProperty, unlockConfig);
            rewardGeneratorObject.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, CombatSettingPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private static void AddObjectReferenceIfMissing(SerializedProperty arrayProperty, UnityEngine.Object target)
    {
        for (int i = 0; i < arrayProperty.arraySize; i++)
        {
            if (arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue == target)
            {
                return;
            }
        }

        int insertIndex = arrayProperty.arraySize;
        arrayProperty.InsertArrayElementAtIndex(insertIndex);
        arrayProperty.GetArrayElementAtIndex(insertIndex).objectReferenceValue = target;
    }

    private static void SetObjectArray(SerializedProperty arrayProperty, AbilityConfig[] values)
    {
        arrayProperty.arraySize = values.Length;

        for (int i = 0; i < values.Length; i++)
        {
            arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
    }

    private static void SetTierFloatArray(SerializedProperty arrayProperty, params (Tier tier, float min, float max)[] values)
    {
        arrayProperty.arraySize = values.Length;

        for (int i = 0; i < values.Length; i++)
        {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("Tier").intValue = (int)values[i].tier;
            element.FindPropertyRelative("MinValue").floatValue = values[i].min;
            element.FindPropertyRelative("MaxValue").floatValue = values[i].max;
        }
    }

    private static void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string[] segments = folderPath.Split('/');
        string currentPath = segments[0];

        for (int i = 1; i < segments.Length; i++)
        {
            string nextPath = currentPath + "/" + segments[i];
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, segments[i]);
            }

            currentPath = nextPath;
        }
    }
}
