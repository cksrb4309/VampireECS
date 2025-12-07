using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class MatchWidthToHeight : MonoBehaviour
{
    [SerializeField] float aspectRatio = 1.0f;

    [SerializeField] RectTransform.Axis axis = RectTransform.Axis.Horizontal;

    private RectTransform rect;

    private float beforeAmount;
    private void Start()
    {
        beforeAmount = 0;
    }
    void FixedUpdate()
    {
        if (rect == null) rect = GetComponent<RectTransform>();

        float amount = axis == RectTransform.Axis.Horizontal ? rect.rect.height : rect.rect.width;

        if (beforeAmount != amount)
        {
            beforeAmount = amount;

            rect.SetSizeWithCurrentAnchors(axis, amount * aspectRatio);
        }
    }
}
