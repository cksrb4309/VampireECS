using UnityEngine;
using UnityEngine.Rendering;

public class MeteorStrikeImpactView : MonoBehaviour
{
    private LineRenderer strikeRenderer;
    private LineRenderer ringRenderer;

    private float duration;
    private float remainingTime;
    private float strikeHeight;
    private float baseRadius;
    private float strikeWidth;
    private float ringWidth;
    private Color baseColor;
    private int segments;

    public void Initialize(Material sharedMaterial)
    {
        EnsureRenderers();
        ApplyMaterial(sharedMaterial);
    }

    public void Play(
        Vector3 position,
        float radius,
        float playDuration,
        float playStrikeHeight,
        float playStrikeWidth,
        float playRingWidth,
        Color color,
        int circleSegments,
        Material sharedMaterial)
    {
        EnsureRenderers();
        ApplyMaterial(sharedMaterial);

        duration = Mathf.Max(0.05f, playDuration);
        remainingTime = duration;
        strikeHeight = Mathf.Max(1f, playStrikeHeight);
        baseRadius = Mathf.Max(0.1f, radius);
        strikeWidth = playStrikeWidth;
        ringWidth = playRingWidth;
        baseColor = color;
        segments = Mathf.Max(18, circleSegments);

        transform.position = position;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        strikeRenderer.positionCount = 2;
        strikeRenderer.SetPosition(0, new Vector3(0f, strikeHeight, 0f));
        strikeRenderer.SetPosition(1, Vector3.zero);

        RebuildRing(baseRadius * 0.35f);
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

        RebuildRing(Mathf.Lerp(baseRadius * 0.35f, baseRadius, progress));
        ApplyVisual(normalized);

        if (remainingTime > 0f)
        {
            return;
        }

        strikeRenderer.positionCount = 0;
        ringRenderer.positionCount = 0;
        gameObject.SetActive(false);
    }

    private void EnsureRenderers()
    {
        if (strikeRenderer != null && ringRenderer != null)
        {
            return;
        }

        GameObject strikeObject = new GameObject("Strike");
        strikeObject.transform.SetParent(transform, false);
        strikeRenderer = strikeObject.AddComponent<LineRenderer>();
        ConfigureRenderer(strikeRenderer, false);

        GameObject ringObject = new GameObject("Ring");
        ringObject.transform.SetParent(transform, false);
        ringRenderer = ringObject.AddComponent<LineRenderer>();
        ConfigureRenderer(ringRenderer, true);
    }

    private static void ConfigureRenderer(LineRenderer renderer, bool loop)
    {
        renderer.useWorldSpace = false;
        renderer.loop = loop;
        renderer.alignment = LineAlignment.View;
        renderer.textureMode = LineTextureMode.Stretch;
        renderer.widthCurve = AnimationCurve.Constant(0f, 1f, 1f);
        renderer.numCapVertices = 2;
        renderer.numCornerVertices = 4;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.generateLightingData = false;
        renderer.positionCount = 0;
    }

    private void ApplyMaterial(Material sharedMaterial)
    {
        if (sharedMaterial == null)
        {
            return;
        }

        strikeRenderer.sharedMaterial = sharedMaterial;
        ringRenderer.sharedMaterial = sharedMaterial;
    }

    private void RebuildRing(float radius)
    {
        ringRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 point = new Vector3(Mathf.Cos(angle) * radius, 0.04f, Mathf.Sin(angle) * radius);
            ringRenderer.SetPosition(i, point);
        }
    }

    private void ApplyVisual(float normalized)
    {
        Color strikeColor = baseColor;
        strikeColor.a *= normalized;

        Color ringColor = baseColor;
        ringColor.a *= normalized * 0.85f;

        strikeRenderer.startColor = strikeColor;
        strikeRenderer.endColor = strikeColor;
        ringRenderer.startColor = ringColor;
        ringRenderer.endColor = ringColor;

        strikeRenderer.widthMultiplier = Mathf.Max(0.01f, strikeWidth * normalized);
        ringRenderer.widthMultiplier = Mathf.Max(0.01f, ringWidth * normalized);
    }
}
