using System;
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

        var tb = ms.Target.GetComponent<TargetBehaviour>();
        if (tb.type == TargetType.Test)
        {
            var solveInfo = await Task.Run(() =>
            {
                var text = textEditorInputField.text;
                var ass = Compilation.GenerateAssambly(text, false);
                var solveInfoInt = Compilation.GenerateTypeWithSolveMethod(ass);
                return solveInfoInt;
            });

            var classInstance = Activator.CreateInstance(solveInfo.ClassType);
            var result = tb.Test(classInstance, solveInfo.SolveMethodInfo);
            Debug.Log("Test Result: " + result);
        }
        else
        {
            var functions = await Task.Run(() =>
            {
                var text = textEditorInputField.text;
                var ass = Compilation.GenerateAssambly(text, false);
                var funcs = Compilation.GenerateAllMethodsFromAssembly(ass);
                return funcs;
            });

            var script = this.ms.AttachMono(functions);
        }

        Camera.main.backgroundColor = Color.black;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.OnClick();
        }
    }
}
