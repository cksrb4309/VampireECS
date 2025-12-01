using Unity.Collections;
using Unity.Entities;

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