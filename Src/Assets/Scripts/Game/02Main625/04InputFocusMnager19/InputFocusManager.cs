using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFocusManager : MonoBehaviour
{
    private List<InputField> inputs = new List<InputField>();
    private List<TMP_InputField> tmpInputs = new List<TMP_InputField>();

    public void Register(InputField inputField)
    {
        this.inputs.Add(inputField);
    }
    
    public void Register(TMP_InputField inputField)
    {
        this.tmpInputs.Add(inputField);
    }

    public bool SafeToTrigger()
    {
        return inputs.All(x => x.isFocused == false) && tmpInputs.All(x=> x.isFocused == false);
    }
}