using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileSelectionButton: MonoBehaviour
{
    private InputField inputField;
    private Main ms;

    private string filePath;

    private void Start()
    {
        var main = GameObject.Find("Main");
        var rb = main.GetComponent<ReferenceBuffer>();
        this.ms = main.GetComponent<Main>();

        this.inputField = rb.TextEditorInputField;
        
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
    }

    public void SetUp(string path)
    {
        this.filePath = path;
    }

    ///TODO: the self attach functionality show not be here;
    private void OnClick()
    {
        var file = File.ReadAllText(this.filePath);
        var alteredFile = file;

        var type = ms.Target?.GetComponent<TargetBehaviour>().type; 

        if (type != TargetType.Test && type != TargetType.BattleMovement)
        {
            alteredFile = Compilation.AddSelfAttachToSource(file);
        }
        this.inputField.text = alteredFile;
    }
}

