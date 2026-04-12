using System.Collections.Generic;
using UnityEngine;

public class BlackHoleViewManager : MonoBehaviour
{
    public static bool HasInstance => instance != null;

    public static BlackHoleViewManager Instance
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

    [SerializeField] private int initialPoolSize = 6;
    [SerializeField] private int circleSegments = 56;
    [SerializeField] private int spiralSegments = 10;
    [SerializeField] private int spokeCount = 6;
    [SerializeField] private float heightOffset = 0.08f;
    [SerializeField] private float outerLineWidth = 0.18f;
    [SerializeField] private float innerLineWidth = 0.12f;
    [SerializeField] private float coreLineWidth = 0.08f;
    [SerializeField] private Color outerColor = new(0.18f, 0.7f, 1f, 0.9f);
    [SerializeField] private Color innerColor = new(0.65f, 0.9f, 1f, 0.75f);
    [SerializeField] private Color coreColor = new(1f, 0.95f, 0.8f, 0.85f);

    private static BlackHoleViewManager instance;

    private readonly List<LineBlackHoleView> views = new();
    private readonly Stack<int> releasedIds = new();
    private readonly HashSet<int> allocatedIds = new();
    private readonly HashSet<int> seenThisFrame = new();
    private readonly List<int> releaseBuffer = new();

    private Material sharedMaterial;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializeOnLoad()
    {
        instance = null;
    }

    private static void CreateRuntimeInstance()
    {
        GameObject root = new GameObject(nameof(BlackHoleViewManager));
        instance = root.AddComponent<BlackHoleViewManager>();
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

    public void BeginFrameSync()
    {
        seenThisFrame.Clear();
    }

    public int CreateView()
    {
        int id;
        LineBlackHoleView view;

        if (releasedIds.Count > 0)
        {
            id = releasedIds.Pop();
            view = views[id];
        }
        else
        {
            id = views.Count;
            view = CreateViewInternal(id);
            views.Add(view);
        }

        allocatedIds.Add(id);
        view.gameObject.SetActive(true);

        return id;
    }

    public void UpdateView(
        int id,
        Vector3 position,
        float radius,
        float remainingDuration,
        float totalDuration,
        float pullStrength)
    {
        if (id < 0 || id >= views.Count)
        {
            return;
        }

        seenThisFrame.Add(id);

        LineBlackHoleView view = views[id];
        view.SetSnapshot(
            position,
            radius,
            remainingDuration,
            totalDuration,
            pullStrength,
            heightOffset,
            circleSegments,
            spiralSegments,
            spokeCount,
            outerLineWidth,
            innerLineWidth,
            coreLineWidth,
            outerColor,
            innerColor,
            coreColor,
            GetOrCreateMaterial());
    }

    public void EndFrameSync()
    {
        releaseBuffer.Clear();

        foreach (int id in allocatedIds)
        {
            if (seenThisFrame.Contains(id))
            {
                continue;
            }

            releaseBuffer.Add(id);
        }

        for (int i = 0; i < releaseBuffer.Count; i++)
        {
            int id = releaseBuffer[i];
            allocatedIds.Remove(id);
            releasedIds.Push(id);

            if (id >= 0 && id < views.Count)
            {
                views[id].Hide();
            }
        }
    }

    private void WarmPool()
    {
        for (int i = views.Count; i < initialPoolSize; i++)
        {
            LineBlackHoleView view = CreateViewInternal(i);
            views.Add(view);
            releasedIds.Push(i);
        }
    }

    private LineBlackHoleView CreateViewInternal(int id)
    {
        GameObject viewObject = new GameObject($"BlackHoleView_{id}");
        viewObject.transform.SetParent(transform, false);

        LineBlackHoleView view = viewObject.AddComponent<LineBlackHoleView>();
        view.Initialize(GetOrCreateMaterial());
        viewObject.SetActive(false);

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
            Debug.LogError("BlackHoleViewManager could not find a runtime shader for the LineRenderer.");
            return null;
        }

        sharedMaterial = new Material(shader)
        {
            name = "BlackHoleRuntimeMaterial"
        };
        sharedMaterial.enableInstancing = true;

        return sharedMaterial;
    }
}
