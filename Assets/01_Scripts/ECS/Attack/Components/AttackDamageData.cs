using Unity.Entities;

public struct AttackDamageData : IComponentData, IAddable<AttackDamageData>
{
    public float Damage;

    public AttackDamageData Add(AttackDamageData other)
    {
        return new AttackDamageData { Damage = Damage + other.Damage };
    }
    public override string ToString()
    {
        return "AttackDamageData : " + Damage;
    }
}
