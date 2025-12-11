using Unity.Entities;

public struct ProjectileCountData : IComponentData, IAddable<ProjectileCountData>
{
    public int Count;

    public ProjectileCountData Add(ProjectileCountData other)
    {
        return new ProjectileCountData { Count = Count + other.Count };
    }
    public override string ToString()
    {
        return "ProjectileCountData : " + Count;
    }
}
