using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public List<InputActionData> InputActionDatas;

    Dictionary<InputType, InputActionReference> inputActions = new();

    Dictionary<InputType, int> inputActionCounts = new Dictionary<InputType, int>();

    public InputActionReference GetInputAction(InputType inputType)
    {
        if (!inputActions.ContainsKey(inputType))
        {
            //Debug.LogWarning("요청한 InputType에 맞는 InputAction이 없습니다 ! : " + inputType.ToString());

            return null;
        }
        if (inputActionCounts.ContainsKey(inputType)) inputActionCounts[inputType]++;

        else inputActionCounts[inputType] = 1;

        inputActions[inputType].action.Enable();

        return inputActions[inputType];
    }
    public void Release(InputType inputType)
    {
        if (!inputActions.ContainsKey(inputType)) return;
        
        if (--inputActionCounts[inputType] == 0)
        {
            inputActions[inputType].action.Disable();
        }
    }
    public void Awake()
    {
        foreach (InputActionData data in InputActionDatas)
        {
            for (int i = 0; i < data.InputActionEntries.Length; i++)
            {
                if (!inputActions.ContainsKey(data.InputActionEntries[i].inputType))
                {
                    inputActions.Add(data.InputActionEntries[i].inputType, data.InputActionEntries[i].inputActionReference);

                    //Debug.Log("InputType이 추가되었습니다 : " + data.InputActionEntries[i].inputType.ToString());
                }
                else
                {
                    //Debug.LogWarning("중복된 InputType이 존재합니다 : " + data.InputActionEntries[i].inputType.ToString());
                }
            }
        }
    }
    private void OnDisable()
    {
        foreach (var kvp in inputActionCounts)
        {
            if (kvp.Value > 0)
            {
                inputActions[kvp.Key].action.Disable();
            }
        }
    }
}
