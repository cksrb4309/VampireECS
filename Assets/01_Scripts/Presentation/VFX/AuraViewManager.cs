using System.Collections.Generic;
using UnityEngine;

public class AuraViewManager : MonoBehaviour
{
    public static AuraViewManager Instance;

    [SerializeField] private VFXAuraView auraPrefab;

    private readonly List<VFXAuraView> views = new();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializeOnLoad()
    {
        Instance = null;
    }
    void Awake()
    {
        Instance = this;
    }

    public int CreateView()
    {
        var view = Instantiate(auraPrefab);

        view.gameObject.SetActive(true);

        int id = views.Count;

        views.Add(view);

        return id;
    }

    public VFXAuraView GetView(int id)
    {
        return views[id];
    }

    public void RemoveView(int id)
    {
        if (id >= 0 && id < views.Count)
            views[id].gameObject.SetActive(false); // or Destroy if needed
    }
}
