using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using UnityEngine;

/// <summary>
/// The one currently in use!
/// </summary>
public class CodeDOMGraph : MonoBehaviour // works in editor
{
    GameObject target;

    void Start()
    {
        this.target = GameObject.Find("Target");
        this.CompileTest();
    }

    public void CompileTest()
    {
        var path = "C:/Users/ASUS G751JY/Desktop/Scripts/scriptOne.txt";
        var ass = this.GenerateAssemply(path);

        var types = ass.GetTypes();
        foreach (var type in types)
        {
            var method = type.GetMethod("Attach");
            var delegateFunc = (Func<GameObject, MonoBehaviour>)
                Delegate.CreateDelegate(typeof(Func<GameObject, MonoBehaviour>), method);
            var addedComponent = delegateFunc.Invoke(target);
        }
    }

    private Assembly GenerateAssemply(string path)
    {
        //CodeDOM Graph https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/generating-and-compiling-source-code-from-a-codedom-graph
        //https://www.reddit.com/r/Unity3D/comments/8nuybs/how_to_compile_and_execute_code_at_runtime/
        //http://www.arcturuscollective.com/archives/22 Dot Net 2.0 compatability level / works only with 4.5 though

        var source = File.ReadAllText(path);
        Debug.Log(source);
        var provider = new CSharpCodeProvider();
        var param = new CompilerParameters()
        {
            GenerateExecutable = false,
            GenerateInMemory = true
        };
        //param.ReferencedAssemblies.Add("System.dll"); // and any other assemblies you'll need
        //param.ReferencedAssemblies.Add("UnityEngine.dll");

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            param.ReferencedAssemblies.Add(assembly.Location);
        }

        var compilerResult = provider.CompileAssemblyFromSource(param, source);

        // you'll need to look at result.Errors to see if it compiled, otherwise..
        foreach (var error in compilerResult.Errors)
        {
            Debug.Log(error);
        }

        var result = compilerResult.CompiledAssembly;
        return result;
    }


    public void AttachTest()
    {
        //Mono1 test = new Mono1();
        //test.Attach(target);
    }
}
