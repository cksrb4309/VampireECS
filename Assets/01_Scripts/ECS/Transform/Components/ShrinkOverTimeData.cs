using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ShrinkOverTimeData : IComponentData
{
    public float Lifetime;                      // 총 지속 시간
    public float ElapsedTime;                   // 현재까지 경과 시간
    public FixedList32Bytes<float> KeyTimes;    // Lifetime 범위 내 keyframe 시간
    public FixedList32Bytes<float> KeyScales;   // KeyTimes에 대응하는 Scale 값
}