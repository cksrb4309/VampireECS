using Unity.Entities;
using UnityEngine;

public class AbilityPrefabLibraryAuthoring : MonoBehaviour
{
    public GameObject ShooterProjectilePrefab;

    public class AbilityPrefabLibraryBaker : Baker<AbilityPrefabLibraryAuthoring>
    {
        public override void Bake(AbilityPrefabLibraryAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);

            AddComponent(entity, new AbilityPrefabLibrary
            {
                ShooterProjectilePrefab = GetEntity(authoring.ShooterProjectilePrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
