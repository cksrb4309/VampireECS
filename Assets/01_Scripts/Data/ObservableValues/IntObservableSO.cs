using UnityEngine;

[CreateAssetMenu(menuName = "Observable/Int")]
public class IntObservableSO : ScriptableObject
{
    public ObservableValue<int> Observable = new ObservableValue<int>();
}
