using UnityEngine;
using TMPro; // TMP »ç¿ë ½Ã

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // UI Text
    public TextMeshProUGUI timeText; // UI Text

    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (fpsText != null)
        {
            fpsText.text = "FPS: " + (1.0f / deltaTime).ToString();
        }
        timeText.text = "Time : " + Time.time;
    }
}
