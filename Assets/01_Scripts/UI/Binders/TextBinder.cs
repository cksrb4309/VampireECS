using TMPro;
using UnityEngine;

public class TextBinder : MonoBehaviour
{
    public FloatObservableSO ValueFloatSO;
    public IntObservableSO ValueIntSO;

    public TMP_Text Label;

    public string Format = string.Empty;

    private void Start()
    {
        if (ValueFloatSO != null)
        {
            ValueFloatSO.Observable.Subscribe(UpdateText);
        }
        if (ValueIntSO != null)
        {
            ValueIntSO.Observable.Subscribe(UpdateText);
        }
    }
    private void OnDestroy()
    {
        if (ValueFloatSO != null)
        {
            ValueFloatSO.Observable.UnSubscribe(UpdateText);
        }
        if (ValueIntSO != null)
        {
            ValueIntSO.Observable.UnSubscribe(UpdateText);
        }
    }
    private void UpdateText(float v)
    {
        Label.text = Format != string.Empty ? v.ToString(Format) : v.ToString();
    }
    private void UpdateText(int v)
    {
        Label.text = Format != string.Empty ? v.ToString(Format) : v.ToString();
    }
}
