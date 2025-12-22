using UnityEngine;

public class UIBootstrapper : MonoBehaviour
{
    [SerializeField] TierTextColorConfig tierConfig;

    void Awake()
    {
        TierTextColorProvider.Initialize(tierConfig);
    }
}
