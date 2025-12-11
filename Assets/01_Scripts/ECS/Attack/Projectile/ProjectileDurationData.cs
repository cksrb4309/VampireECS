using Unity.Entities;

public struct ProjectileDurationData : IComponentData, IAddable<ProjectileDurationData>
{
    public float Duration;

    public ProjectileDurationData Add(ProjectileDurationData other)
    {
        return new ProjectileDurationData
        {
            Duration = Duration + other.Duration
        };
    }
    public override string ToString()
    {
        return "ProjectileDurationData : " + Duration;
    }
}
