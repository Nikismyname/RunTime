using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CodeApplicator: MonoBehaviour
{
    private InputField textEditorInputField;
    private Main ms;

    public async void Apply()
    {
        if (ms.target == null)
        {
            Debug.Log("You should select a target before compiling script to attach to target!");
            return;
        }

        var tb = ms.target.GetComponent<TargetBehaviour>();
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
            else if (tb.type == TargetType.BattleMovement || tb.type == TargetType.BattleMoveSameDom)
            {
                tb.RegisterAI(assBytes);
                result = "AI Loaded!";
            }

            Debug.Log("Test Result: " + result);
            Debug.Log("ExtPath " + ExtPath);
        }
        else
        {
            var target = this.ms.target;
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
    }
}

