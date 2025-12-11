using UnityEngine;
using UnityEngine.UI;

public class ImageBinder : MonoBehaviour
{
    [SerializeField] FloatObservableSO valueFloatSO;
    [SerializeField] IntObservableSO valueIntSO;

    [SerializeField] FloatObservableSO maxValueFloatSO;
    [SerializeField] IntObservableSO maxValueIntSO;

    [SerializeField] Image fillImage;

    float maxValue = 0;
    float value = 0;

    private void Start()
    {
        if (valueFloatSO != null)
            valueFloatSO.Observable.Subscribe(UpdateImage);
        
        if (valueIntSO != null)
            valueIntSO.Observable.Subscribe(UpdateImage);
        
        if (maxValueFloatSO != null)
            maxValueFloatSO.Observable.Subscribe(SetMaxValue);
        
        if (maxValueIntSO != null)
            maxValueIntSO.Observable.Subscribe(SetMaxValue);
    }
    private void OnDestroy()
    {
        if (valueFloatSO != null)
            valueFloatSO.Observable.UnSubscribe(UpdateImage);
        
        if (valueIntSO != null)
            valueIntSO.Observable.UnSubscribe(UpdateImage);
        
        if (maxValueFloatSO != null)
            maxValueFloatSO.Observable.UnSubscribe(SetMaxValue);
        
        if (maxValueIntSO != null)
            maxValueIntSO.Observable.UnSubscribe(SetMaxValue);
    }

    private void UpdateImage(float v)
    {
        value = v;

        fillImage.fillAmount = v != 0f ? v / maxValue : 0f;
    }
    private void UpdateImage(int v)
    {
        value = (float)v;

        fillImage.fillAmount = v != 0 ? (float)v / maxValue : 0f;
    }
    public void SetMaxValue(float v)
    {
        maxValue = v;

        if (valueFloatSO != null)
        {
            UpdateImage(valueFloatSO.Observable.Value);
        }
        if (valueIntSO != null)
        {
            UpdateImage(valueIntSO.Observable.Value);
        }
    }
    public void SetMaxValue(int value)
    {
        maxValue = (float)value;

        if (valueFloatSO != null)
        {
            UpdateImage(valueFloatSO.Observable.Value);
        }
        if (valueIntSO != null)
        {
            UpdateImage(valueIntSO.Observable.Value);
        }
    }
}
