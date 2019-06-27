using UnityEngine;
using UnityEngine.UI;

public class FileSelectionButton: MonoBehaviour
{
    InputField inputField;

    private string filePath;

    private void Start()
    {
        var rb = GameObject.Find("Main").GetComponent<ReferenceBuffer>();
        this.inputField = rb.textEditorInputField;
        
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
    }

    public void SetUp(string path)
    {
        this.filePath = path;
    } 

    private void OnClick()
    {
        var file = Compilation.ReadFile(this.filePath);
        var alteredFile = Compilation.AddSelfAttachToSource(file);
        this.inputField.text = alteredFile;
    }
}

