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
        if(ms.Target == null)
        {
            Debug.Log("You should select a target before compiling script to attach to target!");
            return;
        }

        var funcs = await Task.Run(() =>
        {
            var text = textEditorInputField.text;
            var ass = Compilation.GenerateAssambly(text, false);
            var functions = Compilation.GenerateAllFunctionsFromAssembpy(ass);
            return functions;
        });

        this.Finish(funcs);
    }

    private void Finish(CompMethodsInAssemblyType functions)
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
