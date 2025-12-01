using Unity.Entities;
using Unity.Transforms;

public class ScalablePrefabBaker : Baker<ScalablePrefabAuthoring>
{
    public override void Bake(ScalablePrefabAuthoring authoring)
    {
        UnityEngine.Debug.Log("단계 확인");
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        // 원래 크기 저장
        AddComponent(entity, new OriginalScaleData { Scale = authoring.transform.localScale });

        // World에서 눈에 안 보이게 만들기
        AddComponent(entity, new LocalTransform { Scale = 0f });
    }
}