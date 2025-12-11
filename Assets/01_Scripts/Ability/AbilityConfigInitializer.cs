using System.Collections.Generic;
using UnityEngine;

public class AbilityConfigInitializer : MonoBehaviour
{
    [SerializeField] List<AbilityConfig> abilities;
    [SerializeField] AbilityRewardGenerator generator;

    public void Awake()
    {
        foreach (var ability in abilities)
        {
            generator.AcquireReward(ability);
        }
    }
}