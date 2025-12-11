using Unity.Entities;
using Unity.Mathematics;

public struct PlayerMoveData : IComponentData
{
    public float3 Velocity;

    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float AngularSpeed;
}
