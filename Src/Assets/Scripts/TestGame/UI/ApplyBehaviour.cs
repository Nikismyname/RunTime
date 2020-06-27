using UnityEngine;
using UnityEngine.UI;

public class ApplyBehaviour : MonoBehaviour
{
    private ShowCodeBehaviour showCode;
    private InputFocusManager inputFocusManager;

    void Start()
    {
        this.showCode = ReferenceBuffer.Instance.ShowCode;
        this.inputFocusManager = ReferenceBuffer.Instance.focusManager;
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(CompileTextAndSendToTarget);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && this.inputFocusManager.SafeToTrigger())
        {
            this.CompileTextAndSendToTarget();
        }
    }

    private async void CompileTextAndSendToTarget()
    {
        Camera.main.backgroundColor = Color.cyan;
        string text = showCode.GetText();
        await ReferenceBuffer.Instance.capp.ApplyToSelectedTarget(text);
        Camera.main.backgroundColor = Color.black;
    }
}























































//private async void CompileText()
//{
//    if(ms.target == null)
//    {
//        Debug.Log("You should select a target before compiling script to attach to target!");
//        return;
//    }

//    var tb = ms.target.GetComponent<TargetBehaviour>();
//    if (tb.type == TargetType.Test || tb.type == TargetType.BattleMovement || tb.type == TargetType.BattleMoveSameDom)
//    {
//        string ExtPath = "";
//        var assBytes = await Task.Run(() =>
//        {
//            var text = showCode.GetText();
//            var path = Compilation.GenerateAssemblyToFile(text);
//            ExtPath = path;
//            var bytes = File.ReadAllBytes(path);
//            Debug.Log("Path: " + path);
//            File.Delete(path);
//            return bytes;
//        });

//        string result = string.Empty;

//        if (tb.type == TargetType.Test)
//        {
//            result = (await tb.Test(assBytes)).ToString();
//        }
//        else if(tb.type == TargetType.BattleMovement || tb.type == TargetType.BattleMoveSameDom)
//        {
//            tb.RegisterAI(assBytes);
//            result = "AI Loaded!";
//        }

//        Debug.Log("Test Result: " + result);
//        Debug.Log("ExtPath " + ExtPath);
//        //Compilation.FinalTest(ExtPath); ///Works does not load assemblies to main;
//    }
//    else
//    {
//        var target = this.ms.target;
//        string text = showCode.GetText();
//        text = Compilation.AddSelfAttachToSource(text);
//        var functions = await Task.Run(() =>
//        {
//            Assembly ass = Compilation.GenerateAssemblyInMemory(text, false);
//            var funcs = Compilation.GenerateAllMethodsFromAssembly(ass);
//            return funcs;
//        });

//        var script = this.ms.AttachRuntimeMono(target, functions, text);
//    }
//}