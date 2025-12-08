using Unity.Entities;

public struct ExperienceGainEvent : IComponentData
{
    //public Entity Target;     // 경험치를 받을 타겟
    public int Amount;
}
