using UnityEngine;
using TMPro; // TMP »ç¿ë ½Ã

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // UI Text

    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        if (fpsText != null)
            fpsText.text = $"FPS: {fps:0.}";
    }
}
