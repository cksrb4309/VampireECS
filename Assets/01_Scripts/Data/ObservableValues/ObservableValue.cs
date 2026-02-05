using System;
using UnityEngine;

public class ObservableValue<T> where T : struct
{
    [SerializeField] private T _value;

    event Action<T> OnValueChanged = null;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;

                OnValueChanged?.Invoke(value);
            }
        }
    }
    public void Subscribe(Action<T> onValueChanged)
    {
        onValueChanged.Invoke(Value);

        OnValueChanged += onValueChanged;
    }
    public void UnSubscribe(Action<T> onValueChanged)
    {
        OnValueChanged -= onValueChanged;
    }
}
