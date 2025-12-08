using UnityEngine;

[CreateAssetMenu(menuName = "Observable/Float")]
public class FloatObservableSO : ScriptableObject
{
    public ObservableValue<float> Observable = new ObservableValue<float>();
}
