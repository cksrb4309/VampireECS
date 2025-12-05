using Unity.Entities;
using Unity.Mathematics;

public struct DynamicRotationData : IComponentData
{
    public quaternion BaseRotation;          // 바라보는 방향(절대값)
    public float3 RotationAxis;              // 로컬 회전축 (비정규화 가능)
    public float AngularSpeed;               // rad/sec (라디안 단위)

    public quaternion AccumulatedRotation;   // 누적 회전 (초기값 = quaternion.identity)
}
