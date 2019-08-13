using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to input field, this script regesteres the InputField componenet with the input 
/// field manager and destroys itself.
/// </summary>
public class InputFocusRegister : MonoBehaviour
{
    private void Start()
    {
        var field = gameObject.GetComponent<InputField>();
        var main = GameObject.Find("Main");
        var inputFocusManager = main.GetComponent<InputFocusManager>();
        inputFocusManager.Register(field); 
        Destroy(this);
    }
}

