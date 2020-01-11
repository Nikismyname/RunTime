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
        if (tb.type == TargetType.Test || tb.type == TargetType.BattleMovement || tb.type == TargetType.BattleMoveSameDom)
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

            string result = string.Empty;

            if (tb.type == TargetType.Test)
            {
                result = (await tb.Test(assBytes)).ToString();
            }
            else if(tb.type == TargetType.BattleMovement || tb.type == TargetType.BattleMoveSameDom)
            {
                tb.RegisterAI(assBytes);
                result = "AI Loaded!";
            }

            Debug.Log("Test Result: " + result);
            Debug.Log("ExtPath " + ExtPath);
            //Compilation.FinalTest(ExtPath); ///Works does not load assemblies to main;
        }
        else
        {
            var target = this.ms.Target; 
            var functions = await Task.Run(() =>
            {
                var text = textEditorInputField.text;
                text = Compilation.AddSelfAttachToSource(text);
                var ass = Compilation.GenerateAssemblyInMemory(text, false);
                var funcs = Compilation.GenerateAllMethodsFromAssembly(ass);
                return funcs;
            });

            var script = this.ms.AttachRuntimeMono(target, functions);
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
