using UnityEngine;

public class DynamicRotationAuthoring : MonoBehaviour
{
    [Header("회전 속도")]
    public float RotationSpeed = 180f;

    [Header("회전 방향 전환 설정")]
    [Tooltip("새로운 회전 방향을 결정하는 최소 주기(초)")]
    public float MinDirectionChangeInterval = 1.0f;

    [Tooltip("새로운 회전 방향을 결정하는 최대 주기(초)")]
    public float MaxDirectionChangeInterval = 3.0f;

    [Tooltip("회전 방향을 전환할 때 부드럽게 전환되는 속도")]
    public float DirectionLerpSpeed = 2.0f;

    [Header("초기 회전 범위")]
    [Tooltip("초기에 랜덤 방향 회전을 할지 여부")]
    public bool RandomInitialDirection = true;
}
