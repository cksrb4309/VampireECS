using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class BoomerangProjectilePrefabSetup
{
    private const string MaterialFolderPath = "Assets/02_Art/Materials/Props";
    private const string MeshFolderPath = "Assets/02_Art/Meshes/Props";
    private const string MaterialAssetPath = MaterialFolderPath + "/BoomerangProjectile.mat";
    private const string MeshAssetPath = MeshFolderPath + "/BoomerangProjectileMesh.asset";
    private const string PrefabAssetPath = "Assets/06_Prefabs/Player/Projectile/BoomerangProjectile.prefab";
    private const string EntitiesSubScenePath = "Assets/07_Scenes/Test_Combat/EntitiesSubScene.unity";

    [MenuItem("Tools/Codex Validation/Setup Boomerang Projectile Prefab")]
    public static void SetupBoomerangProjectilePrefabMenu()
    {
        SetupBoomerangProjectilePrefab(logToConsole: true);
    }

    public static void SetupBoomerangProjectilePrefabBatch()
    {
        SetupBoomerangProjectilePrefab(logToConsole: true);
    }

    private static void SetupBoomerangProjectilePrefab(bool logToConsole)
    {
        EnsureFolderExists(MaterialFolderPath);
        EnsureFolderExists(MeshFolderPath);

        Material material = CreateOrUpdateMaterial();
        Mesh mesh = CreateOrUpdateMesh();
        GameObject prefab = CreateOrUpdatePrefab(mesh, material);

        ConnectAbilityPrefabLibrary(prefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (logToConsole)
        {
            Debug.Log($"[CodexSetup] Boomerang projectile prefab configured. Material='{MaterialAssetPath}', Mesh='{MeshAssetPath}', Prefab='{PrefabAssetPath}'.");
        }
    }

    private static Material CreateOrUpdateMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            throw new InvalidOperationException("Universal Render Pipeline/Lit shader was not found.");
        }

        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialAssetPath);
        if (material == null)
        {
            material = new Material(shader)
            {
                name = "BoomerangProjectile"
            };

            AssetDatabase.CreateAsset(material, MaterialAssetPath);
        }

        material.shader = shader;
        material.enableInstancing = true;
        material.SetColor("_BaseColor", new Color(0.12f, 0.9f, 0.28f, 1f));
        material.SetColor("_EmissionColor", new Color(0.06f, 0.3f, 0.1f, 1f));
        material.SetFloat("_Smoothness", 0.18f);

        EditorUtility.SetDirty(material);
        return material;
    }

    private static Mesh CreateOrUpdateMesh()
    {
        Mesh meshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(MeshAssetPath);
        Mesh generatedMesh = BuildBoomerangMesh();

        if (meshAsset == null)
        {
            AssetDatabase.CreateAsset(generatedMesh, MeshAssetPath);
            return generatedMesh;
        }

        meshAsset.Clear();
        meshAsset.vertices = generatedMesh.vertices;
        meshAsset.triangles = generatedMesh.triangles;
        meshAsset.normals = generatedMesh.normals;
        meshAsset.uv = generatedMesh.uv;
        meshAsset.bounds = generatedMesh.bounds;
        meshAsset.RecalculateBounds();
        meshAsset.RecalculateNormals();

        EditorUtility.SetDirty(meshAsset);
        UnityEngine.Object.DestroyImmediate(generatedMesh);

        return meshAsset;
    }

    private static Mesh BuildBoomerangMesh()
    {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
        UnityEngine.Object.DestroyImmediate(primitive);

        CombineInstance[] combines = new CombineInstance[3];
        combines[0] = new CombineInstance
        {
            mesh = cubeMesh,
            transform = Matrix4x4.TRS(
                new Vector3(-0.22f, 0f, 0.12f),
                Quaternion.Euler(0f, -38f, 0f),
                new Vector3(0.16f, 0.07f, 0.86f))
        };
        combines[1] = new CombineInstance
        {
            mesh = cubeMesh,
            transform = Matrix4x4.TRS(
                new Vector3(0.22f, 0f, 0.12f),
                Quaternion.Euler(0f, 38f, 0f),
                new Vector3(0.16f, 0.07f, 0.86f))
        };
        combines[2] = new CombineInstance
        {
            mesh = cubeMesh,
            transform = Matrix4x4.TRS(
                new Vector3(0f, 0f, -0.05f),
                Quaternion.identity,
                new Vector3(0.12f, 0.055f, 0.26f))
        };

        Mesh mesh = new Mesh
        {
            name = "BoomerangProjectileMesh"
        };

        mesh.CombineMeshes(combines, true, true, false);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private static GameObject CreateOrUpdatePrefab(Mesh mesh, Material material)
    {
        GameObject root = new GameObject("BoomerangProjectile");

        try
        {
            MeshFilter meshFilter = root.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            MeshRenderer meshRenderer = root.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            root.AddComponent<ProjectileAuthoring>();

            GameObject prefabRoot = PrefabUtility.SaveAsPrefabAsset(root, PrefabAssetPath);
            if (prefabRoot == null)
            {
                throw new InvalidOperationException($"Failed to save prefab '{PrefabAssetPath}'.");
            }

            return prefabRoot;
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static void ConnectAbilityPrefabLibrary(GameObject boomerangProjectilePrefab)
    {
        Scene subScene = default;
        bool openedScene = false;

        try
        {
            subScene = EditorSceneManager.GetSceneByPath(EntitiesSubScenePath);
            if (!subScene.IsValid() || !subScene.isLoaded)
            {
                subScene = EditorSceneManager.OpenScene(EntitiesSubScenePath, OpenSceneMode.Additive);
                openedScene = true;
            }

            AbilityPrefabLibraryAuthoring authoring = FindAbilityPrefabLibraryAuthoring(subScene);
            if (authoring == null)
            {
                throw new InvalidOperationException($"AbilityPrefabLibraryAuthoring was not found in '{EntitiesSubScenePath}'.");
            }

            authoring.BoomerangProjectilePrefab = boomerangProjectilePrefab;
            EditorUtility.SetDirty(authoring);
            EditorSceneManager.MarkSceneDirty(subScene);
            EditorSceneManager.SaveScene(subScene);
        }
        finally
        {
            if (openedScene && subScene.IsValid() && subScene.isLoaded)
            {
                EditorSceneManager.CloseScene(subScene, true);
            }
        }
    }

    private static AbilityPrefabLibraryAuthoring FindAbilityPrefabLibraryAuthoring(Scene scene)
    {
        foreach (GameObject rootGameObject in scene.GetRootGameObjects())
        {
            AbilityPrefabLibraryAuthoring authoring = rootGameObject.GetComponentInChildren<AbilityPrefabLibraryAuthoring>(true);
            if (authoring != null)
            {
                return authoring;
            }
        }

        return null;
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
