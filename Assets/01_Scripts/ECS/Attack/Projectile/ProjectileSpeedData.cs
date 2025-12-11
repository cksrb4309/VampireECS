using Unity.Entities;

public struct ProjectileSpeedData : IComponentData, IAddable<ProjectileSpeedData>
{
    public float Speed;

    public ProjectileSpeedData Add(ProjectileSpeedData other)
    {
        return new ProjectileSpeedData
        {
            Speed = Speed + other.Speed
        };
    }
    public override string ToString()
    {
        return "ProjectileSpeedData : " + Speed;
    }
}
