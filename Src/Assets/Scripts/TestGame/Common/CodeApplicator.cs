using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class CodeApplicator
{
    private Main ms;

    public CodeApplicator()
    {
        this.ms = ReferenceBuffer.Instance.ms;
    }

    public async void ApplyToSelectedTarget(string text)
    {
        if (ms.target == null)
        {
            Debug.Log("You should select a target before compiling script to attach to target!");
            return;
        }

        var tb = ms.target.GetComponent<TargetBehaviour>();
        if (tb.type == TargetType.Test || tb.type == TargetType.BattleMovement || tb.type == TargetType.BattleMoveSameDom)
        {
            byte[] assBytes = await Task.Run(() =>
            {
                var bytes = OtherAppDomainCompile(text);
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
        }
        else
        {
            var target = this.ms.target;

            var functions = await Task.Run(() =>
            {
                var funcs = this.SameAppDomainCompile(text, true);
                return funcs;
            });

            var script = this.ms.AttachRuntimeMono(target, functions, text);
        }
    }

    public CompMethodsInAssemblyType SameAppDomainCompile(string code, bool addSelfAttach = true)
    {
        if (addSelfAttach)
        {
            code = Compilation.AddSelfAttachToSource(code);
        }
        Assembly ass = Compilation.GenerateAssemblyInMemory(code, false);
        CompMethodsInAssemblyType funcs = Compilation.GenerateAllMethodsFromAssembly(ass);
        return funcs;
    }

    public byte[] OtherAppDomainCompile(string code)
    {
        string path = Compilation.GenerateAssemblyToFile(code);
        var bytes = File.ReadAllBytes(path);
        File.Delete(path);
        return bytes;
    }
}

