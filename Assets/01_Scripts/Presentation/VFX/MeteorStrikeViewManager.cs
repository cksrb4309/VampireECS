using System.Collections.Generic;
using UnityEngine;

public class MeteorStrikeViewManager : MonoBehaviour
{
    public static MeteorStrikeViewManager Instance
    {
        get
        {
            if (instance == null)
            {
                CreateRuntimeInstance();
            }

            return instance;
        }
    }

    [SerializeField] private int initialTelegraphPoolSize = 8;
    [SerializeField] private int initialImpactPoolSize = 8;
    [SerializeField] private int circleSegments = 48;
    [SerializeField] private float telegraphHeightOffset = 0.05f;
    [SerializeField] private float telegraphLineWidth = 0.14f;
    [SerializeField] private float impactLineWidth = 0.18f;
    [SerializeField] private float impactRingWidth = 0.2f;
    [SerializeField] private Color telegraphColor = new(1f, 0.45f, 0.2f, 0.9f);
    [SerializeField] private Color impactColor = new(1f, 0.85f, 0.4f, 1f);

    private static MeteorStrikeViewManager instance;

    private readonly List<MeteorStrikeTelegraphView> telegraphPool = new();
    private readonly List<MeteorStrikeImpactView> impactPool = new();

    private Material sharedMaterial;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializeOnLoad()
    {
        instance = null;
    }

    private static void CreateRuntimeInstance()
    {
        GameObject root = new GameObject(nameof(MeteorStrikeViewManager));
        instance = root.AddComponent<MeteorStrikeViewManager>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        WarmPools();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        if (sharedMaterial != null)
        {
            Destroy(sharedMaterial);
            sharedMaterial = null;
        }
    }

    public void EmitTelegraph(Vector3 position, float radius, float duration)
    {
        MeteorStrikeTelegraphView view = GetOrCreateTelegraphView();
        view.Play(
            position,
            radius,
            duration,
            telegraphHeightOffset,
            telegraphLineWidth,
            telegraphColor,
            circleSegments,
            GetOrCreateMaterial());
    }

    public void EmitImpact(Vector3 position, float radius, float duration, float strikeHeight)
    {
        MeteorStrikeImpactView view = GetOrCreateImpactView();
        view.Play(
            position,
            radius,
            duration,
            strikeHeight,
            impactLineWidth,
            impactRingWidth,
            impactColor,
            circleSegments,
            GetOrCreateMaterial());
    }

    private void WarmPools()
    {
        for (int i = telegraphPool.Count; i < initialTelegraphPoolSize; i++)
        {
            CreateTelegraphView();
        }

        for (int i = impactPool.Count; i < initialImpactPoolSize; i++)
        {
            CreateImpactView();
        }
    }

    private MeteorStrikeTelegraphView GetOrCreateTelegraphView()
    {
        for (int i = 0; i < telegraphPool.Count; i++)
        {
            if (!telegraphPool[i].gameObject.activeSelf)
            {
                return telegraphPool[i];
            }
        }

        return CreateTelegraphView();
    }

    private MeteorStrikeImpactView GetOrCreateImpactView()
    {
        for (int i = 0; i < impactPool.Count; i++)
        {
            if (!impactPool[i].gameObject.activeSelf)
            {
                return impactPool[i];
            }
        }

        return CreateImpactView();
    }

    private MeteorStrikeTelegraphView CreateTelegraphView()
    {
        GameObject viewObject = new GameObject($"MeteorTelegraphView_{telegraphPool.Count}");
        viewObject.transform.SetParent(transform, false);

        MeteorStrikeTelegraphView view = viewObject.AddComponent<MeteorStrikeTelegraphView>();
        view.Initialize(GetOrCreateMaterial());
        viewObject.SetActive(false);

        telegraphPool.Add(view);

        return view;
    }

    private MeteorStrikeImpactView CreateImpactView()
    {
        GameObject viewObject = new GameObject($"MeteorImpactView_{impactPool.Count}");
        viewObject.transform.SetParent(transform, false);

        MeteorStrikeImpactView view = viewObject.AddComponent<MeteorStrikeImpactView>();
        view.Initialize(GetOrCreateMaterial());
        viewObject.SetActive(false);

        impactPool.Add(view);

        return view;
    }

    private Material GetOrCreateMaterial()
    {
        if (sharedMaterial != null)
        {
            return sharedMaterial;
        }

        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        if (shader == null)
        {
            Debug.LogError("MeteorStrikeViewManager could not find a runtime shader for the LineRenderer.");
            return null;
        }

        sharedMaterial = new Material(shader)
        {
            name = "MeteorStrikeRuntimeMaterial"
        };
        sharedMaterial.enableInstancing = true;

        return sharedMaterial;
    }
}
