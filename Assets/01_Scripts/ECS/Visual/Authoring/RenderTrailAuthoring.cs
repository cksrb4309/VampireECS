using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
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

    public class RenderTrailBaker : Baker<RenderTrailAuthoring>
    {
        public override void Bake(RenderTrailAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            FixedList32Bytes<float> times = new FixedList32Bytes<float>();
            FixedList32Bytes<float> scales = new FixedList32Bytes<float>();

            foreach (var t in authoring.KeyTimes) times.Add(t);
            foreach (var s in authoring.KeyScales) scales.Add(s);

            AddComponent(entity, new RenderTrailData
            {
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                SpawnDistance = authoring.SpawnDistance,
                LifeTime = authoring.Lifetime,

                KeyTimes = times,
                KeyScales = scales
            });
        }
    }
}
