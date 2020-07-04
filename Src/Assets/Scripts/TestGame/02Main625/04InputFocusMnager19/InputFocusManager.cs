using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InputFocusManager : MonoBehaviour
{
    List<InputField> inputs = new List<InputField>();

    public void Register(InputField inputField)
    {
        this.inputs.Add(inputField);
    }

    public bool SafeToTrigger()
    {
        return inputs.All(x => x.isFocused == false);
    }
}


