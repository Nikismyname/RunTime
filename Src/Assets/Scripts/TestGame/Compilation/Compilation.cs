using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class Compilation
{
    public const string path = "C:/Users/ASUS G751JY/Unity/ScriptDonor/ScriptDonor/Assets/TestScripts";

    public static Assembly GenerateAssambly(string str, bool isPath)
    {
        var source = "";
        if (isPath)
        {
            source = File.ReadAllText(str);
        }
        else
        {
            source = str;
        }

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
            Debug.Log("Compilation Error " + error);
        }

        var result = compilerResult.CompiledAssembly;
        return result;
    }

    public static CompFunctionsInAssemblyType GenerateAllFunctions(Assembly ass)
    {
        var result = new CompFunctionsInAssemblyType();

        var types = ass.GetTypes();
        //var allAtaches = new List<Func<GameObject, MonoBehaviour>>();

        if (types.Length == 0)
        {
            Debug.Log("No types found in assembly!");
            return null;
        }

        if (types.Length > 1)
        {
            Debug.Log("More than one type found in assembly!");
            return null;
        }

        var type = types[0];
        var method = type.GetMethod("Attach");
        var attachFunk = (Func<GameObject, MonoBehaviour>)
        Delegate.CreateDelegate(typeof(Func<GameObject, MonoBehaviour>), method);
        result.Attach = attachFunk;

        var flags =
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |
            BindingFlags.Instance;

        if (Settings.CompilationIncludePrivate)
        {
            flags = flags | BindingFlags.NonPublic;
        }

        if (Settings.CompilationIncludeStatic)
        {
            flags = flags | BindingFlags.Static;
        }

        var otherMethods = type.GetMethods(flags)
            .Where(x => x.Name != "Attach");

        var methodInfos = new Dictionary<string, CompMethodInfoWIthParams>();
        foreach (var item in otherMethods)
        {
            var parameterInfos = item.GetParameters();
            var parameters = new List<UiParameterWithType>();

            foreach (var parInfo in parameterInfos)
            {
                parameters.Add(new UiParameterWithType
                {
                    Name = parInfo.Name,
                    Type = parInfo.ParameterType
                });
            }

            methodInfos[item.Name] = new CompMethodInfoWIthParams
            {
                MethodInfo = item,
                Parameters = parameters.ToArray(),
            };
        }

        result.MethodInfos = methodInfos;
        result.TypeName = type.Name;

        return result;
    }

    public static string AddSelfAttachToSource(string source)
    {
        source = source.Trim();
        List<string> lines = source.Split(new char[] { '\n' }).ToList();
        int[] originalIndentation = new int[lines.Count];

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (line.Length <= 0)
            {
                continue;
            }

            var counter = 0;
            var c = line[counter];
            while (counter < line.Length && c == ' ')
            {
                counter++;
                c = line[counter];
            }

            originalIndentation[i] = counter;
        }

        //cleaning the lines so we can find the opening bracket;
        lines = lines.Select(x => x.Trim()).ToList();

        if (lines[lines.Count - 1] != "}")
        {
            Debug.LogError("File does not end with \"}\"");
            return null;
        }

        //public class Test1 : MonoBehaviour

        var openingBraketLineNumber = lines.IndexOf("{");
        if (openingBraketLineNumber == -1)
        {
            Debug.Log("Can not find opening bracket braket!");
            return null;
        }

        var classLine = lines[openingBraketLineNumber - 1];

        var tokens = classLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var indexOfClass = Array.IndexOf(tokens, "class");

        if (indexOfClass == -1)
        {
            Debug.Log("Can not find class keyword in the class declaration line!");
            return null;
        }

        if (tokens.Length - 1 == indexOfClass)
        {
            Debug.Log("Can not find the name of the class after the class keyword!");
            return null;
        }

        var name = string.Empty;
        var namePart = tokens[indexOfClass + 1];
        if (namePart.Contains(":"))
        {
            name = namePart.Substring(0, namePart.IndexOf(":"));
        }
        else
        {
            name = namePart;
        }

        if (name.Length == 0)
        {
            Debug.Log("Name not found!");
            return null;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i] = new string(' ', originalIndentation[i]) + lines[i];
        }

        var toInsrt = $@"
    public static {name} Attach(GameObject obj)
    {{
        return obj.AddComponent<{name}>();
    }}
";

        lines.Insert(lines.Count - 1, toInsrt);

        var result = string.Join("\n", lines);
        return result;
    }

    public static string[] GetAllCsFiles(string path)
    {
        var result = new List<string>();

        var allFiles = Directory.GetFiles(path);

        foreach (var item in allFiles)
        {
            if (item.EndsWith(".cs"))
            {
                result.Add(item);
            }
        }

        return result.ToArray();
    }

    public static string ReadFile(string path)
    {
        return File.ReadAllText(path);
    }
}
