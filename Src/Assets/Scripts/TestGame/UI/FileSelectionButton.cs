using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileSelectionButton: MonoBehaviour
{
    private ShowCodeBehaviour showCode;
    private Main ms;

    private string filePath;

    private void Start()
    {
        var main = GameObject.Find("Main");
        var rb = main.GetComponent<ReferenceBuffer>();
        this.ms = main.GetComponent<Main>();

        this.showCode = rb.ShowCode;
        
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
    }

    public void SetUp(string path)
    {
        this.filePath = path;
    }

    private void OnClick()
    {
        var file = File.ReadAllText(this.filePath);
        var alteredFile = file;
        this.showCode.SetText(alteredFile);
    }
}

