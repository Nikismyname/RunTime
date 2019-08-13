using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ApplyBehaviour : MonoBehaviour
{
    private InputField textEditorInputField;
    private Camera myCamera;
    private Main ms;
    private InputFocusManager inputFocusManager;

    void Start()
    {
        this.myCamera = Camera.main;

        this.textEditorInputField = GameObject.Find("TextEditor").GetComponent<InputField>();
        var main = GameObject.Find("Main");
        this.ms = main.GetComponent<Main>();
        this.inputFocusManager = main.GetComponent<InputFocusManager>();

        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(CompileTextAndSendToTarget);

    }

    private void CompileTextAndSendToTarget()
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

        var tb = ms.Target.GetComponent<TargetBehaviour>();
        if (tb.type == TargetType.Test)
        {
            string ExtPath = "";
            var assBytes = await Task.Run(() =>
            {
                var text = textEditorInputField.text;
                var path = Compilation.GenerateAssemblyToFile(text);
                ExtPath = path;
                var bytes = File.ReadAllBytes(path);
                Debug.Log("Path: " + path);
                File.Delete(path);
                return bytes;
            });

            var result = tb.Test(assBytes);
            Debug.Log("Test Result: " + result);
            Debug.Log("ExtPath " + ExtPath);
            //Compilation.FinalTest(ExtPath); ///Works does not load assemblies to main;
        }
        else
        {
            var functions = await Task.Run(() =>
            {
                var text = textEditorInputField.text;
                var ass = Compilation.GenerateAssemblyInMemory(text, false);
                var funcs = Compilation.GenerateAllMethodsFromAssembly(ass);
                return funcs;
            });

            var script = this.ms.AttachMono(functions);
        }

        Camera.main.backgroundColor = Color.black;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && this.inputFocusManager.SafeToTrigger())
        {
            this.CompileTextAndSendToTarget();
        }
    }
}
