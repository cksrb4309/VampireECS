using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class MeteorStrikeTelegraphView : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private float duration;
    private float remainingTime;
    private float baseRadius;
    private float heightOffset;
    private float baseWidth;
    private Color baseColor;
    private int segments;

    public void Initialize(Material sharedMaterial)
    {
        EnsureLineRenderer();

        if (sharedMaterial != null)
        {
            lineRenderer.sharedMaterial = sharedMaterial;
        }
    }

    public void Play(
        Vector3 position,
        float radius,
        float playDuration,
        float positionHeightOffset,
        float width,
        Color color,
        int circleSegments,
        Material sharedMaterial)
    {
        EnsureLineRenderer();

        if (sharedMaterial != null)
        {
            lineRenderer.sharedMaterial = sharedMaterial;
        }

        duration = Mathf.Max(0.05f, playDuration);
        remainingTime = duration;
        baseRadius = Mathf.Max(0.05f, radius);
        heightOffset = positionHeightOffset;
        baseWidth = width;
        baseColor = color;
        segments = Mathf.Max(12, circleSegments);

        transform.position = position + Vector3.up * heightOffset;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        RebuildCircle(baseRadius);
        ApplyVisual(1f);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (remainingTime <= 0f)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        float normalized = Mathf.Clamp01(remainingTime / duration);
        float progress = 1f - normalized;
        float radiusScale = 1f + progress * 0.12f;

        RebuildCircle(baseRadius * radiusScale);
        ApplyVisual(normalized);

        if (remainingTime > 0f)
        {
            return;
        }

        lineRenderer.positionCount = 0;
        gameObject.SetActive(false);
    }

    private void EnsureLineRenderer()
    {
        if (lineRenderer != null)
        {
            return;
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.widthCurve = AnimationCurve.Constant(0f, 1f, 1f);
        lineRenderer.numCapVertices = 2;
        lineRenderer.numCornerVertices = 4;
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.generateLightingData = false;
        lineRenderer.positionCount = 0;
    }

    private void RebuildCircle(float radius)
    {
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 point = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            lineRenderer.SetPosition(i, point);
        }
    }

    private void ApplyVisual(float normalized)
    {
        float pulse = 0.85f + Mathf.Sin((1f - normalized) * Mathf.PI * 8f) * 0.15f;

        Color color = baseColor;
        color.a *= Mathf.Lerp(0.35f, 1f, 1f - normalized);

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.widthMultiplier = Mathf.Max(0.01f, baseWidth * pulse);
    }
}
