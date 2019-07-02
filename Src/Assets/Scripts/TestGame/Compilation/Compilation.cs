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
    public const string testPath = "C:/Users/ASUS G751JY/Unity/ScriptDonor/ScriptDonor/Assets/Tests/Solutions";

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
            GenerateInMemory = true,
        };

        ///Manualy Provide available assemblies
        //param.ReferencedAssemblies.Add("System.dll"); // and any other assemblies you'll need
        //param.ReferencedAssemblies.Add("UnityEngine.dll");

        ///Add all assemblies form Current Domain?
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

        ///I beleve it returns null if compilation had Error (not Warning)
        var result = compilerResult.CompiledAssembly;
        return result;
    }

    public static Type GetSingleTypeFromAssembly(Assembly ass)
    {
        var types = ass.GetTypes();

        if (types.Length == 0)
        {
            Debug.Log("No types found in assembly!");
            return null;
        }

        ///Will be posible to extract more that one in the future
        if (types.Length > 1)
        {
            Debug.Log("More than one type found in assembly!");
            return null;
        }

        var type = types[0];

        return type;
    }

    public static CompTypeWithSolveMehtodInfo GenerateTypeWithSolveMethod(Assembly ass)
    {
        var type = GetSingleTypeFromAssembly(ass);
        var solveMehtod = type.GetMethod("Solve");
        if (solveMehtod == null)
        {
            Debug.Log("Solve Method Not Found!");
            return null;
        }

        var result = new CompTypeWithSolveMehtodInfo
        {
            ClassType = type,
            SolveMethodInfo = solveMehtod,
        };

        return result;
    }

    public static CompMethodsInAssemblyType GenerateAllMethodsFromAssembly(Assembly ass)
    {
        var type = GetSingleTypeFromAssembly(ass);
        return GenerateAllMethodsFromType(type);
    }

    public static CompMethodsInAssemblyType GenerateAllMethodsFromType(Type type)
    {
        var result = new CompMethodsInAssemblyType();

        ///Getting the attach method and converting it to Funk that attaches to GameObject and returns the MonoBehaviour
        var method = type.GetMethod("Attach");

        Func<GameObject, MonoBehaviour> attachFunk = null;
        if (method == null)
        {
            //Debug.Log("There is not attach method on Type: " + type.Name);
        }
        else
        {
            attachFunk = (Func<GameObject, MonoBehaviour>)
                Delegate.CreateDelegate(typeof(Func<GameObject, MonoBehaviour>), method);
            result.Attach = attachFunk;
        }

        /// Setting the flags for method extraction
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
        ///...

        ///Non Attach methods are gathered here
        var otherMethods = type.GetMethods(flags)
            .Where(x => x.Name != "Attach");

        ///Gathering the information for method parameters
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
        ///...

        result.MethodInfos = methodInfos;
        result.TypeName = type.Name;

        return result;
    }

    public static string AddSelfAttachToSource(string source)
    {
        var lines = GetTrimedSourceLines(source).ToList(); 

        if (lines[lines.Count - 1] != "}")
        {
            Debug.LogError("File does not end with \"}\"");
            return null;
        }

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

    ///Trims source and removes comments  
    public static List<string> GetTrimedSourceLines(string source)
    {
        var lines = source
            .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim(' ', '\t', '\r', '\t', '\v'))
            .ToList();

        /// removing all empty lines
        lines = lines.Where(x => x != "").ToList();

        ///removing /**/ comments 
        ///collection the loactions of all /**/ comments
        var openingBrakets = new List<CommentInfo>();
        var closingBrackets = new List<CommentInfo>();

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            var indexOfOpeningBraket = line.IndexOf("/*");
            while (indexOfOpeningBraket != -1)
            {
                openingBrakets.Add(new CommentInfo(i, indexOfOpeningBraket));
                indexOfOpeningBraket = line.IndexOf("/*", indexOfOpeningBraket + 1);
            }

            var indexOfClosingBraket = line.IndexOf("*/");
            while (indexOfClosingBraket != -1)
            {
                closingBrackets.Add(new CommentInfo(i, indexOfClosingBraket));
                indexOfClosingBraket = line.IndexOf("*/", indexOfClosingBraket + 1);
            }
        }

        ///If we have /**/ comments, we romove them from the source
        if (openingBrakets.Count > 0 && closingBrackets.Count > 0)
        {
            ///Comment brackets mismatch
            if (openingBrakets.First().CompareTo(closingBrackets.First()) > 0)
            {
                Debug.Log("First closing comment bracket before first opening comment bracket!");
                return null;
            }

            if (openingBrakets.Last().CompareTo(closingBrackets.Last()) > 0)
            {
                Debug.Log("Last opening comment bracket is after last closing comment bracket!");
                return null;
            }
            ///...

            var openingBraket = openingBrakets.First();

            while (openingBraket != null)
            {
                var closingBracket = closingBrackets.Where(x=> x.CompareTo(openingBraket)>0).FirstOrDefault();

                if (closingBracket == null)
                {
                    Debug.Log("Braket mismath");
                    return null; 
                }

                ///delete the whole comented lines
                if(closingBracket.Line - openingBraket.Line > 1)
                {
                    for (int i = openingBraket.Line + 1; i < closingBracket.Line; i++)
                    {
                        lines[i] = "";
                    }
                }
                ///...

                /*Delete the comment from partially commented lines*/
                Debug.Log(openingBraket + "|" + closingBracket);
                Debug.Log(closingBracket.Col + 2 + " " + lines[closingBracket.Line].Length);

                ///If lines open and close same line -> we cut out the comment and append spaces that will 
                ///later be removed so the data for the next comment on the same line is the same 
                ///as at the point where it was collected
                if (closingBracket.Line == openingBraket.Line)
                {
                    var lineLenght = lines[openingBraket.Line].Length; 
                    lines[openingBraket.Line] = lines[openingBraket.Line].Substring(0, openingBraket.Col) + lines[closingBracket.Line].Substring(closingBracket.Col + 2);
                    var newLineLenght = lines[openingBraket.Line].Length;
                    var dif = lineLenght - newLineLenght;
                    lines[openingBraket.Line] = new string(' ', dif) + lines[openingBraket.Line];
                }
                else
                {
                    lines[closingBracket.Line] = lines[closingBracket.Line].Substring(closingBracket.Col + 2);
                    lines[openingBraket.Line] = lines[openingBraket.Line].Substring(0, openingBraket.Col);
                }
                
                openingBraket = openingBrakets.Where(x => x.CompareTo(closingBracket) > 0).FirstOrDefault();
            }
        }

        ///removing line comments
        for (int i = 0; i < lines.Count; i++)
        {
            var indexOfLineComment = lines[i].IndexOf("//");
            if (indexOfLineComment != -1)
            {
                lines[i] = lines[i].Substring(0, indexOfLineComment);
            }
        }
        ///...

        ///trim to remove the spaces added for the multiple comments per line cenario
        lines = lines.Select(x => x.Trim(' ', '\t', '\r', '\t', '\v')).ToList();
        ///final trim to remove line comments that started at the beginning of the line
        lines = lines.Where(x => x != "").ToList();

        return lines;
    }

    class CommentInfo : IComparable<CommentInfo>
    {
        public CommentInfo(int line, int col)
        {
            Line = line;
            Col = col;
        }

        public int Line { get; set; }
        public int Col { get; set; }

        public int CompareTo(CommentInfo other)
        {
            var colComp = this.Line.CompareTo(other.Line);
            if (colComp == 0)
            {
                return this.Col.CompareTo(other.Col);
            }
            else
            {
                return colComp;
            }
        }

        public override string ToString()
        {
            return $"Line: {this.Line} Col: {this.Col}"; 
        }
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


#region OLD_CODE
//if (closingBracket.Col + 2 == lines[closingBracket.Line].Length)
//{
//    lines[closingBracket.Line] = "";
//}
//else
//{

//public static string AddSelfAttachToSource(string source)
//{
//    source = source.Trim();
//    List<string> lines = source.Split(new char[] { '\n' }).ToList();
//    int[] originalIndentation = new int[lines.Count];

//    ///saving the original line indentations
//    for (int i = 0; i < lines.Count; i++)
//    {
//        var line = lines[i];

//        if (line.Length <= 0)
//        {
//            continue;
//        }

//        var counter = 0;
//        var c = line[counter];
//        while (counter < line.Length && c == ' ')
//        {
//            counter++;
//            c = line[counter];
//        }

//        originalIndentation[i] = counter;
//    }

//    //triming the lines so we can find the opening bracket;
//    lines = lines.Select(x => x.Trim()).ToList();

//    if (lines[lines.Count - 1] != "}")
//    {
//        Debug.LogError("File does not end with \"}\"");
//        return null;
//    }

//    //public class Test1 : MonoBehaviour

//    var openingBraketLineNumber = lines.IndexOf("{");
//    if (openingBraketLineNumber == -1)
//    {
//        Debug.Log("Can not find opening bracket braket!");
//        return null;
//    }

//    var classLine = lines[openingBraketLineNumber - 1];

//    var tokens = classLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//    var indexOfClass = Array.IndexOf(tokens, "class");

//    if (indexOfClass == -1)
//    {
//        Debug.Log("Can not find class keyword in the class declaration line!");
//        return null;
//    }

//    if (tokens.Length - 1 == indexOfClass)
//    {
//        Debug.Log("Can not find the name of the class after the class keyword!");
//        return null;
//    }

//    var name = string.Empty;
//    var namePart = tokens[indexOfClass + 1];
//    if (namePart.Contains(":"))
//    {
//        name = namePart.Substring(0, namePart.IndexOf(":"));
//    }
//    else
//    {
//        name = namePart;
//    }

//    if (name.Length == 0)
//    {
//        Debug.Log("Name not found!");
//        return null;
//    }

//    for (int i = 0; i < lines.Count; i++)
//    {
//        lines[i] = new string(' ', originalIndentation[i]) + lines[i];
//    }

//    var toInsrt = $@"
//    public static {name} Attach(GameObject obj)
//    {{
//        return obj.AddComponent<{name}>();
//    }}
//";

//    lines.Insert(lines.Count - 1, toInsrt);

//    var result = string.Join("\n", lines);
//    return result;
//}
#endregion