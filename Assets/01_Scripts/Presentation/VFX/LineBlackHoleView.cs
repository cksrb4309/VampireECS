using UnityEngine;
using UnityEngine.Rendering;

public class LineBlackHoleView : MonoBehaviour
{
    private LineRenderer outerRingRenderer;
    private LineRenderer innerRingRenderer;
    private LineRenderer coreRingRenderer;
    private LineRenderer spokeRenderer;

    private float radius;
    private float remainingDuration;
    private float totalDuration;
    private float pullStrength;
    private float heightOffset;
    private float outerLineWidth;
    private float innerLineWidth;
    private float coreLineWidth;
    private int circleSegments;
    private int spiralSegments;
    private int spokeCount;

    private Color outerColor;
    private Color innerColor;
    private Color coreColor;

    private float animationTime;

    public void Initialize(Material sharedMaterial)
    {
        EnsureRenderers();
        ApplyMaterial(sharedMaterial);
    }

    public void SetSnapshot(
        Vector3 position,
        float currentRadius,
        float currentRemainingDuration,
        float currentTotalDuration,
        float currentPullStrength,
        float currentHeightOffset,
        int currentCircleSegments,
        int currentSpiralSegments,
        int currentSpokeCount,
        float currentOuterLineWidth,
        float currentInnerLineWidth,
        float currentCoreLineWidth,
        Color currentOuterColor,
        Color currentInnerColor,
        Color currentCoreColor,
        Material sharedMaterial)
    {
        EnsureRenderers();
        ApplyMaterial(sharedMaterial);

        radius = Mathf.Max(0.1f, currentRadius);
        remainingDuration = Mathf.Max(0f, currentRemainingDuration);
        totalDuration = Mathf.Max(0.01f, currentTotalDuration);
        pullStrength = Mathf.Max(0f, currentPullStrength);
        heightOffset = currentHeightOffset;
        circleSegments = Mathf.Max(24, currentCircleSegments);
        spiralSegments = Mathf.Max(4, currentSpiralSegments);
        spokeCount = Mathf.Max(3, currentSpokeCount);
        outerLineWidth = currentOuterLineWidth;
        innerLineWidth = currentInnerLineWidth;
        coreLineWidth = currentCoreLineWidth;
        outerColor = currentOuterColor;
        innerColor = currentInnerColor;
        coreColor = currentCoreColor;

        transform.position = position + Vector3.up * heightOffset;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (outerRingRenderer != null)
        {
            outerRingRenderer.positionCount = 0;
        }

        if (innerRingRenderer != null)
        {
            innerRingRenderer.positionCount = 0;
        }

        if (coreRingRenderer != null)
        {
            coreRingRenderer.positionCount = 0;
        }

        if (spokeRenderer != null)
        {
            spokeRenderer.positionCount = 0;
        }

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        animationTime += Time.deltaTime;

        float normalizedLife = Mathf.Clamp01(remainingDuration / totalDuration);
        float age = 1f - normalizedLife;

        float outerRadius = radius * (1f + Mathf.Sin(animationTime * 2.2f) * 0.04f);
        float innerRadius = radius * (0.66f + Mathf.Sin(animationTime * 3.6f) * 0.03f);
        float coreRadius = radius * (0.28f + Mathf.Sin(animationTime * 5.1f) * 0.025f);
        float spokeRadius = radius * Mathf.Lerp(0.7f, 1f, Mathf.Sin(animationTime * 1.7f) * 0.5f + 0.5f);
        float swirlStrength = radius * (0.18f + Mathf.Min(0.12f, pullStrength * 0.01f));

        RebuildCircle(outerRingRenderer, outerRadius, circleSegments, animationTime * -0.8f);
        RebuildCircle(innerRingRenderer, innerRadius, circleSegments, animationTime * 1.6f);
        RebuildCircle(coreRingRenderer, coreRadius, circleSegments, animationTime * -2.4f);
        RebuildSpokes(spokeRenderer, spokeRadius, swirlStrength, spokeCount, spiralSegments, animationTime);

        ApplyVisuals(normalizedLife, age);
    }

    private void EnsureRenderers()
    {
        if (outerRingRenderer != null)
        {
            return;
        }

        outerRingRenderer = CreateRenderer("OuterRing", true);
        innerRingRenderer = CreateRenderer("InnerRing", true);
        coreRingRenderer = CreateRenderer("CoreRing", true);
        spokeRenderer = CreateRenderer("Spokes", false);
    }

    private LineRenderer CreateRenderer(string objectName, bool loop)
    {
        GameObject child = new GameObject(objectName);
        child.transform.SetParent(transform, false);

        LineRenderer renderer = child.AddComponent<LineRenderer>();
        renderer.useWorldSpace = false;
        renderer.loop = loop;
        renderer.alignment = LineAlignment.View;
        renderer.textureMode = LineTextureMode.Stretch;
        renderer.widthCurve = AnimationCurve.Constant(0f, 1f, 1f);
        renderer.numCapVertices = 3;
        renderer.numCornerVertices = 4;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.generateLightingData = false;
        renderer.positionCount = 0;

        return renderer;
    }

    private void ApplyMaterial(Material sharedMaterial)
    {
        if (sharedMaterial == null)
        {
            return;
        }

        outerRingRenderer.sharedMaterial = sharedMaterial;
        innerRingRenderer.sharedMaterial = sharedMaterial;
        coreRingRenderer.sharedMaterial = sharedMaterial;
        spokeRenderer.sharedMaterial = sharedMaterial;
    }

    private static void RebuildCircle(LineRenderer renderer, float circleRadius, int segments, float phase)
    {
        renderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)segments;
            float angle = t * Mathf.PI * 2f + phase;
            renderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * circleRadius, 0f, Mathf.Sin(angle) * circleRadius));
        }
    }

    private static void RebuildSpokes(
        LineRenderer renderer,
        float spokeRadius,
        float swirlStrength,
        int spokeCount,
        int spiralSegments,
        float animationTime)
    {
        int totalSegments = Mathf.Max(spiralSegments * spokeCount, 8);
        renderer.positionCount = totalSegments + 1;

        for (int segmentIndex = 0; segmentIndex <= totalSegments; segmentIndex++)
        {
            float t = segmentIndex / (float)totalSegments;
            float radialScale = 1f - t;
            float localRadius = Mathf.Lerp(spokeRadius, 0.04f, t);
            float angle = animationTime * 1.3f + t * Mathf.PI * 2f * spokeCount;
            float swirlOffset = Mathf.Sin(animationTime * 2.4f + t * 10f) * swirlStrength * radialScale;

            Vector3 point = new Vector3(
                Mathf.Cos(angle) * localRadius,
                0.01f * radialScale,
                Mathf.Sin(angle) * localRadius);

            Vector3 tangent = new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            point += tangent * swirlOffset;

            renderer.SetPosition(segmentIndex, point);
        }
    }

    private void ApplyVisuals(float normalizedLife, float age)
    {
        float fadeIn = Mathf.Clamp01(age / 0.15f);
        float fadeOut = Mathf.Clamp01(normalizedLife / 0.2f);
        float alphaScale = Mathf.Min(fadeIn, fadeOut);
        float pulse = 0.9f + Mathf.Sin(animationTime * 4.5f) * 0.1f;

        Color outer = outerColor;
        outer.a *= alphaScale;

        Color inner = innerColor;
        inner.a *= alphaScale;

        Color core = coreColor;
        core.a *= alphaScale;

        outerRingRenderer.startColor = outer;
        outerRingRenderer.endColor = outer;
        outerRingRenderer.widthMultiplier = Mathf.Max(0.01f, outerLineWidth * pulse);

        innerRingRenderer.startColor = inner;
        innerRingRenderer.endColor = inner;
        innerRingRenderer.widthMultiplier = Mathf.Max(0.01f, innerLineWidth * (1.05f - pulse * 0.15f));

        coreRingRenderer.startColor = core;
        coreRingRenderer.endColor = core;
        coreRingRenderer.widthMultiplier = Mathf.Max(0.01f, coreLineWidth * (1.1f + Mathf.Sin(animationTime * 7f) * 0.12f));

        Color spoke = inner;
        spoke.a *= 0.7f;
        spokeRenderer.startColor = spoke;
        spokeRenderer.endColor = core;
        spokeRenderer.widthMultiplier = Mathf.Max(0.01f, coreLineWidth * 0.75f);
    }
}
