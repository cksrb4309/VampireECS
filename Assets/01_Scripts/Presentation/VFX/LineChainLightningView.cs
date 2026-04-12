using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class LineChainLightningView : MonoBehaviour
{
    private readonly List<Vector3> pointBuffer = new(32);

    private LineRenderer lineRenderer;

    private float duration;
    private float remainingTime;
    private float baseWidth;
    private float heightOffset;
    private int subdivisionsPerSegment;
    private float jitterAmplitude;

    private Color baseColor;

    public void Initialize(Material sharedMaterial)
    {
        EnsureLineRenderer();

        if (sharedMaterial != null)
        {
            lineRenderer.sharedMaterial = sharedMaterial;
        }
    }

    public void Play(
        FixedList512Bytes<ChainLightningSegment> segments,
        Material sharedMaterial,
        float playDuration,
        float width,
        Color color,
        float positionHeightOffset,
        int segmentSubdivisions,
        float segmentJitterAmplitude)
    {
        if (segments.Length == 0)
        {
            return;
        }

        EnsureLineRenderer();

        if (sharedMaterial != null)
        {
            lineRenderer.sharedMaterial = sharedMaterial;
        }

        duration = Mathf.Max(0.01f, playDuration);
        remainingTime = duration;
        baseWidth = width;
        baseColor = color;
        heightOffset = positionHeightOffset;
        subdivisionsPerSegment = Mathf.Max(0, segmentSubdivisions);
        jitterAmplitude = Mathf.Max(0f, segmentJitterAmplitude);

        RebuildPoints(segments);
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
        lineRenderer.useWorldSpace = true;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.widthCurve = AnimationCurve.Constant(0f, 1f, 1f);
        lineRenderer.numCapVertices = 3;
        lineRenderer.numCornerVertices = 2;
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.generateLightingData = false;
        lineRenderer.positionCount = 0;
    }

    private void RebuildPoints(FixedList512Bytes<ChainLightningSegment> segments)
    {
        pointBuffer.Clear();

        Vector3 offset = Vector3.up * heightOffset;

        for (int i = 0; i < segments.Length; i++)
        {
            Vector3 from = ToVector3(segments[i].From) + offset;
            Vector3 to = ToVector3(segments[i].To) + offset;

            AppendSegmentPoints(from, to, i == 0);
        }

        lineRenderer.positionCount = pointBuffer.Count;

        for (int i = 0; i < pointBuffer.Count; i++)
        {
            lineRenderer.SetPosition(i, pointBuffer[i]);
        }
    }

    private void AppendSegmentPoints(Vector3 from, Vector3 to, bool includeStart)
    {
        if (includeStart)
        {
            pointBuffer.Add(from);
        }

        Vector3 direction = to - from;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            pointBuffer.Add(to);
            return;
        }

        Vector3 side = Vector3.Cross(direction.normalized, Vector3.up);

        if (side.sqrMagnitude <= 0.0001f)
        {
            side = Vector3.Cross(direction.normalized, Vector3.forward);
        }

        side.Normalize();

        for (int i = 1; i <= subdivisionsPerSegment; i++)
        {
            float t = i / (subdivisionsPerSegment + 1f);
            float centerWeight = 1f - Mathf.Abs((t - 0.5f) * 2f);

            Vector3 point = Vector3.Lerp(from, to, t);
            point += side * UnityEngine.Random.Range(-jitterAmplitude, jitterAmplitude) * centerWeight;

            pointBuffer.Add(point);
        }

        pointBuffer.Add(to);
    }

    private void ApplyVisual(float normalized)
    {
        Color color = baseColor;
        color.a *= normalized;

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.widthMultiplier = Mathf.Max(0.01f, baseWidth * normalized);
    }

    private static Vector3 ToVector3(float3 value)
    {
        return value;
    }
}
