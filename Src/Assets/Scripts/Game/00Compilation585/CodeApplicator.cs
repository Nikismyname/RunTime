using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class CodeApplicator
{
    private TargetManagerBehaviour ms;

    public CodeApplicator()
    {
        this.ms = ReferenceBuffer.Instance.ms;
    }

    public async Task ApplyToSelectedTarget(string text, GameObject target = null)
    {
        try
        {
            if (this.ms.selector.Target == null && target == null)
            {
                Debug.Log("You should select a target before compiling script to attach to target!");
                return;
            }

            TargetBehaviour tb = null; 
            if (this.ms.selector.Target != null)
            {
                tb = this.ms.selector.Target.GetComponent<TargetBehaviour>();
            }

            if (tb != null && (tb.type == TargetType.Test || tb.type == TargetType.BattleMovement || tb.type == TargetType.BattleMoveSameDom))
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
                if (target == null)
                {
                    target = this.ms.selector.Target;
                }

                var functions = await Task.Run(() =>
                {
                    var funcs = this.SameAppDomainCompile(text, true);
                    return funcs;
                });

                var script = this.ms.attacher.AttachRuntimeMono(target, functions, text);
            } 
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    public CompMethodsInAssemblyType SameAppDomainCompile(string code, bool addSelfAttach = true)
    {
        if (addSelfAttach)
        {
            code = SourceManipulation.AddSelfAttachToSource(code);
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

