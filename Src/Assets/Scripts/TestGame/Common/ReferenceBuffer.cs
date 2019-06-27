using UnityEngine;
using UnityEngine.UI;

public class ReferenceBuffer : MonoBehaviour
{
    public InputField textEditorInputField;  

    private void Awake()
    {
        this.textEditorInputField = GameObject.Find("TextEditor").GetComponent<InputField>();
    }
}
