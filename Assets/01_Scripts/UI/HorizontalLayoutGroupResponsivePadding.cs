using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(HorizontalLayoutGroup))]
[ExecuteAlways]
public class HorizontalLayoutGroupResponsivePadding : MonoBehaviour
{
    public enum PaddingBase
    {
        None,   // None: left/right -> width, top/bottom -> height
        Width,  // 모든 패딩을 width 기준으로 계산
        Height  // 모든 패딩을 height 기준으로 계산
    }

    [Header("Base Calculation")]
    [Tooltip("padding 계산 기준 축 (None = 좌/우는 width, 상/하 는 height)")]
    public PaddingBase baseAxis = PaddingBase.Width;

    [Header("Individual Padding Ratios")]
    [Tooltip("Left padding 계산용 비율 (base에 따라 width 또는 height에 곱해짐)")]
    public float ratioLeft = 0.05f;

    [Tooltip("Right padding 계산용 비율 (base에 따라 width 또는 height에 곱해짐)")]
    public float ratioRight = 0.05f;

    [Tooltip("Top padding 계산용 비율 (base에 따라 width 또는 height에 곱해짐)")]
    public float ratioTop = 0.05f;

    [Tooltip("Bottom padding 계산용 비율 (base에 따라 width 또는 height에 곱해짐)")]
    public float ratioBottom = 0.05f;

    [Header("Optional Limits")]
    public int minPadding = 0;
    public int maxPadding = 300;

    private HorizontalLayoutGroup hlg;
    private RectTransform rect;

    void Awake()
    {
        hlg = GetComponent<HorizontalLayoutGroup>();
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        ApplyPadding();
    }

#if UNITY_EDITOR
    void Update()
    {
        // 에디터에서 즉시 반영
        if (!Application.isPlaying)
            ApplyPadding();
    }
#endif

    void OnRectTransformDimensionsChange()
    {
        if (isActiveAndEnabled)
            ApplyPadding();
    }

    private void ApplyPadding()
    {
        if (hlg == null || rect == null)
            return;

        float width = rect.rect.width;
        float height = rect.rect.height;

        int left, right, top, bottom;

        switch (baseAxis)
        {
            case PaddingBase.Width:
                left = Mathf.RoundToInt(width * ratioLeft);
                right = Mathf.RoundToInt(width * ratioRight);
                top = Mathf.RoundToInt(width * ratioTop);
                bottom = Mathf.RoundToInt(width * ratioBottom);
                break;

            case PaddingBase.Height:
                left = Mathf.RoundToInt(height * ratioLeft);
                right = Mathf.RoundToInt(height * ratioRight);
                top = Mathf.RoundToInt(height * ratioTop);
                bottom = Mathf.RoundToInt(height * ratioBottom);
                break;

            case PaddingBase.None:
            default:
                // None: left/right은 width에, top/bottom은 height에 영향받음
                left = Mathf.RoundToInt(width * ratioLeft);
                right = Mathf.RoundToInt(width * ratioRight);
                top = Mathf.RoundToInt(height * ratioTop);
                bottom = Mathf.RoundToInt(height * ratioBottom);
                break;
        }

        // 클램프
        left = Mathf.Clamp(left, minPadding, maxPadding);
        right = Mathf.Clamp(right, minPadding, maxPadding);
        top = Mathf.Clamp(top, minPadding, maxPadding);
        bottom = Mathf.Clamp(bottom, minPadding, maxPadding);

        // 적용
        hlg.padding.left = left;
        hlg.padding.right = right;
        hlg.padding.top = top;
        hlg.padding.bottom = bottom;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // 1) Editor PlayerLoop 갱신 예약 (보조)
            EditorApplication.QueuePlayerLoopUpdate();

            // 2) 즉시 레이아웃 재계산 (강제)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            // 3) SceneView 강제 리페인트
            SceneView.RepaintAll();
        }
#endif
    }
}