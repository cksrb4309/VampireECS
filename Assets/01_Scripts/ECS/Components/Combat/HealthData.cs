using Unity.Entities;

public struct HealthData : IComponentData/*, IEnableableComponent*/
{
    public float Current;
    public float Max;

}
