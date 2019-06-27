using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ApplyBehaviour : MonoBehaviour
{
    private InputField textEditorInputField;
    private Camera myCamera;
    private Main ms;

    void Start()
    {
        this.myCamera = Camera.main;

        this.textEditorInputField = GameObject.Find("TextEditor").GetComponent<InputField>();
        this.ms = GameObject.Find("Main").GetComponent<Main>();

        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Camera.main.backgroundColor = Color.cyan;
        this.CompileText();
    }

    private async void CompileText()
    {
        var funcs = await Task.Run(() =>
        {
            var text = textEditorInputField.text;
            var ass = Compilation.GenerateAssambly(text, false);
            var functions = Compilation.GenerateAllFunctions(ass);
            return functions;
        });

        this.Finish(funcs);
    }

    private void Finish(CompFunctionsInAssemblyType functions)
    {
        var script = this.ms.AttachMono(functions);
        Camera.main.backgroundColor = Color.black;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            this.OnClick();
        }
    }
}
