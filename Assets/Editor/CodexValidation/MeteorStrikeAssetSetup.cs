using System;
using UnityEditor;
using UnityEngine;

public static class MeteorStrikeAssetSetup
{
    private const string StatsFolderPath = "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats";
    private const string UnlockFolderPath = "Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock";
    private const string StatsAssetPath = StatsFolderPath + "/MeteorStrikeStatsConfig.asset";
    private const string UnlockAssetPath = UnlockFolderPath + "/UnlockMeteorStrikeConfig.asset";
    private const string CombatSettingPrefabPath = "Assets/06_Prefabs/Scene/CombatSetting.prefab";

    [MenuItem("Tools/Codex Validation/Setup Meteor Strike Assets")]
    public static void SetupMeteorStrikeAssetsMenu()
    {
        SetupMeteorStrikeAssets(logToConsole: true);
    }

    public static void SetupMeteorStrikeAssetsBatch()
    {
        SetupMeteorStrikeAssets(logToConsole: true);
    }

    private static void SetupMeteorStrikeAssets(bool logToConsole)
    {
        EnsureFolderExists(StatsFolderPath);
        EnsureFolderExists(UnlockFolderPath);

        MeteorStrikeStatsConfig statsConfig = LoadOrCreateAsset<MeteorStrikeStatsConfig>(StatsAssetPath);
        UnlockMeteorStrikeConfig unlockConfig = LoadOrCreateAsset<UnlockMeteorStrikeConfig>(UnlockAssetPath);

        ConfigureMeteorStrikeStats(statsConfig);
        ConfigureUnlockMeteorStrike(unlockConfig, statsConfig);
        ConnectCombatSettingPrefab(unlockConfig);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (logToConsole)
        {
            Debug.Log($"[CodexSetup] Meteor strike assets configured. Stats='{StatsAssetPath}', Unlock='{UnlockAssetPath}', Prefab='{CombatSettingPrefabPath}'.");
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

    private static void ConfigureMeteorStrikeStats(MeteorStrikeStatsConfig config)
    {
        SerializedObject serializedObject = new SerializedObject(config);

        serializedObject.FindProperty("m_Name").stringValue = "MeteorStrikeStatsConfig";
        serializedObject.FindProperty("MaxStack").intValue = 200;
        serializedObject.FindProperty("IsStackable").boolValue = true;
        serializedObject.FindProperty("Icon").objectReferenceValue = null;
        serializedObject.FindProperty("CurrentTier").intValue = (int)Tier.None;

        SetObjectArray(serializedObject.FindProperty("prerequisitesAbilities"), Array.Empty<AbilityConfig>());

        SetTierFloatArray(serializedObject.FindProperty("damageValue"),
            (Tier.Bronze, 5f, 10f),
            (Tier.Silver, 10f, 20f),
            (Tier.Gold, 20f, 35f));

        SetTierFloatArray(serializedObject.FindProperty("attackSpeedValue"),
            (Tier.Bronze, 0.05f, 0.1f),
            (Tier.Silver, 0.1f, 0.2f),
            (Tier.Gold, 0.2f, 0.3f));

        SetTierFloatArray(serializedObject.FindProperty("acquireRadiusValue"),
            (Tier.Bronze, 1f, 2f),
            (Tier.Silver, 2f, 3f),
            (Tier.Gold, 3f, 5f));

        SetTierFloatArray(serializedObject.FindProperty("impactRadiusValue"),
            (Tier.Bronze, 0.25f, 0.5f),
            (Tier.Silver, 0.5f, 0.75f),
            (Tier.Gold, 0.75f, 1.25f));

        SetTierFloatArray(serializedObject.FindProperty("impactDelayValue"),
            (Tier.Bronze, -0.1f, -0.05f),
            (Tier.Silver, -0.18f, -0.1f),
            (Tier.Gold, -0.3f, -0.18f));

        SetTierIntArray(serializedObject.FindProperty("meteorCountValue"),
            (Tier.Bronze, 1, 1),
            (Tier.Silver, 1, 2),
            (Tier.Gold, 2, 3));

        SetTierFloatArray(serializedObject.FindProperty("scatterRadiusValue"),
            (Tier.Bronze, 0.4f, 0.8f),
            (Tier.Silver, 0.8f, 1.2f),
            (Tier.Gold, 1.2f, 1.8f));

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(config);
    }

    private static void ConfigureUnlockMeteorStrike(UnlockMeteorStrikeConfig config, MeteorStrikeStatsConfig prerequisiteStats)
    {
        SerializedObject serializedObject = new SerializedObject(config);

        serializedObject.FindProperty("m_Name").stringValue = "UnlockMeteorStrikeConfig";
        serializedObject.FindProperty("MaxStack").intValue = 1;
        serializedObject.FindProperty("IsStackable").boolValue = false;
        serializedObject.FindProperty("Icon").objectReferenceValue = null;
        serializedObject.FindProperty("CurrentTier").intValue = (int)Tier.None;
        serializedObject.FindProperty("fixedTier").intValue = (int)Tier.Gold;
        serializedObject.FindProperty("description").stringValue = "가장 가까운 적 위치에 지연 폭발 메테오를 떨어뜨립니다";

        SetObjectArray(
            serializedObject.FindProperty("prerequisitesAbilities"),
            new AbilityConfig[] { prerequisiteStats });

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(config);
    }

    private static void ConnectCombatSettingPrefab(UnlockMeteorStrikeConfig unlockConfig)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(CombatSettingPrefabPath);

        try
        {
            AbilityConfigInitializer initializer = prefabRoot.GetComponentInChildren<AbilityConfigInitializer>(true);
            if (initializer == null)
            {
                throw new InvalidOperationException($"AbilityConfigInitializer not found in '{CombatSettingPrefabPath}'.");
            }

            SerializedObject serializedObject = new SerializedObject(initializer);
            SerializedProperty abilitiesProperty = serializedObject.FindProperty("abilities");

            AddObjectReferenceIfMissing(abilitiesProperty, unlockConfig);

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
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

    private static void SetTierIntArray(SerializedProperty arrayProperty, params (Tier tier, int min, int max)[] values)
    {
        arrayProperty.arraySize = values.Length;

        for (int i = 0; i < values.Length; i++)
        {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("Tier").intValue = (int)values[i].tier;
            element.FindPropertyRelative("MinValue").intValue = values[i].min;
            element.FindPropertyRelative("MaxValue").intValue = values[i].max;
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
