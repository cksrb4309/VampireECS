using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderTrailAuthoring : MonoBehaviour
{
    public GameObject Prefab;

    public float SpawnDistance = 1.5f;
    public float Lifetime = 0.5f;
    public float Scale = 1.0f;

    public List<float> KeyTimes;
    public List<float> KeyScales;

    [Button]
    public void Normalize()
    {
        float maxValue = float.MinValue;

        maxValue = KeyTimes.Max();

        // Normalize → 0 ~ Lifetime 범위로 조정
        for (int i = 0; i < KeyTimes.Count; i++)
        {
            float normalized01 = KeyTimes[i] / maxValue; // 0~1 압축
            KeyTimes[i] = normalized01 * Lifetime;      // 0~Lifetime 확장
        }

        maxValue = KeyScales.Max();

        // Normalize → 0 ~ Scale 범위로 조정
        for (int i = 0; i < KeyScales.Count; i++)
        {
            float normalized01 = KeyScales[i] / maxValue; // 0~1 압축
            KeyScales[i] = normalized01 * Scale;      // 0~Lifetime 확장
        }
    }
}
