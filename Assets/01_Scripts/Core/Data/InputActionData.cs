using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputActionData", menuName = "Core/Data/InputActionData")]
public class InputActionData : ScriptableObject
{
    [SerializeField] InputActionEntry[] inputActionEntries;

    public InputActionEntry[] InputActionEntries => inputActionEntries;

    [Serializable]
    public class InputActionEntry
    {
        public InputType inputType;
        public InputActionReference inputActionReference;
    }
}
