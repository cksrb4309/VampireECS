using Unity.Entities;

public struct AttackSpeedData : IComponentData, IAddable<AttackSpeedData>
{
    public float AttackSpeed;

    public AttackSpeedData Add(AttackSpeedData other)
    {
        return new AttackSpeedData
        {
            AttackSpeed = AttackSpeed + other.AttackSpeed
        };
    }
    public override string ToString()
    {
        return "AttackSpeedData : " + AttackSpeed;
    }
}
