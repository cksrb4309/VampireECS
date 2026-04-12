using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ChainLightningViewManager : MonoBehaviour
{
    public static ChainLightningViewManager Instance
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

    [SerializeField] private int initialPoolSize = 8;
    [SerializeField] private float defaultDuration = 0.12f;
    [SerializeField] private float lineWidth = 0.22f;
    [SerializeField] private float heightOffset = 0.7f;
    [SerializeField] private int subdivisionsPerSegment = 3;
    [SerializeField] private float jitterAmplitude = 0.18f;
    [SerializeField] private Color lightningColor = new(0.58f, 0.88f, 1f, 1f);

    private static ChainLightningViewManager instance;

    private readonly List<LineChainLightningView> viewPool = new();

    private Material sharedMaterial;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializeOnLoad()
    {
        instance = null;
    }

    private static void CreateRuntimeInstance()
    {
        GameObject root = new GameObject(nameof(ChainLightningViewManager));
        instance = root.AddComponent<ChainLightningViewManager>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        WarmPool();
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

    public void Emit(FixedList512Bytes<ChainLightningSegment> segments, float duration)
    {
        if (segments.Length == 0)
        {
            return;
        }

        LineChainLightningView view = GetOrCreateView();

        view.Play(
            segments,
            GetOrCreateMaterial(),
            duration > 0f ? duration : defaultDuration,
            lineWidth,
            lightningColor,
            heightOffset,
            subdivisionsPerSegment,
            jitterAmplitude);
    }

    private void WarmPool()
    {
        for (int i = viewPool.Count; i < initialPoolSize; i++)
        {
            CreateView();
        }
    }

    private LineChainLightningView GetOrCreateView()
    {
        for (int i = 0; i < viewPool.Count; i++)
        {
            if (!viewPool[i].gameObject.activeSelf)
            {
                return viewPool[i];
            }
        }

        return CreateView();
    }

    private LineChainLightningView CreateView()
    {
        GameObject viewObject = new GameObject($"ChainLightningView_{viewPool.Count}");
        viewObject.transform.SetParent(transform, false);

        LineChainLightningView view = viewObject.AddComponent<LineChainLightningView>();
        view.Initialize(GetOrCreateMaterial());
        viewObject.SetActive(false);

        viewPool.Add(view);

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
            Debug.LogError("ChainLightningViewManager could not find a runtime shader for the LineRenderer.");
            return null;
        }

        sharedMaterial = new Material(shader)
        {
            name = "ChainLightningRuntimeMaterial"
        };
        sharedMaterial.enableInstancing = true;

        return sharedMaterial;
    }
}
