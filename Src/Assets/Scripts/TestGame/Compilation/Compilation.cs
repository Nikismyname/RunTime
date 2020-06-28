#region INIT
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using diagnostics = System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Reflection.Emit;
using System.Threading;
using sd = System.Diagnostics;
using System.Runtime.InteropServices;

public static class Compilation
{
    public const string BasePath = "C:/Users/ASUS G751JY/source/repos/11111 Unity/UserCode"; 
    public const string RunTimeAssembliesPath = "C:/Users/ASUS G751JY/source/repos/11111 Unity/RunTime/Src/Assets/Assemblies";
    public const string NewAssembliesLocation = "C:/Users/ASUS G751JY/source/repos/11111 Unity/Assembies";
    public const string testAssemblyFullName = "C:/Users/ASUS G751JY/source/repos/11111 Unity/Assembies/8b15ac31-e824-4ee5-b2a9-3940498e5709.dll";
    public const string testAssemblyName = "8b15ac31-e824-4ee5-b2a9-3940498e5709";

    private static AppDomain testDomain;
    public static Loader Loader { get; set; }
    public static Loader LoaderSameDom { get; set; }
    #endregion

    #region SETTING_UP_TEST_DOMAIN
    /// <summary>
    /// All the setup neccessary to conduct tests in separate assembly
    /// </summary>
    static Compilation()
    {
        /// Separate domain works for tests (anything that sends plane data really),
        /// does not load the code in the main assembly. 
        /// Can not be used to share reflection data however

        testDomain = SetUpAsMain("Test Domain");
        AddNecessaryAssembliesToDomain(testDomain);
        //AddAllCurrentDomainAssembliesToDomain(testDomain);
        Loader = InitLoader(testDomain);
        LoaderSameDom = new Loader(); 
    }

    #region DOMAIN_SET_UP_AS_MAIN
    private static AppDomain SetUpAsMain(string name)
    {
        var domain = AppDomain.CreateDomain(
            name,
            AppDomain.CurrentDomain.Evidence,
            AppDomain.CurrentDomain.SetupInformation);

        return domain;
    }
    #endregion

    #region ADD_NECESSARY_ASSEMBLIES_TO_DOMAIN_CURRENTLY_JUST_LOADER
    /// <summary>
    /// Apperantly it does not matter what is loaded other that the Loader.dll
    /// </summary>
    private static void AddNecessaryAssembliesToDomain(AppDomain domain)
    {
        ///Manualy Provide available assemblies
        var locations = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.Location);
        var assembliesToLoad = new List<string>();

        ///Loads Loader.dll if we are using it (it is .net standart) or else Loader2(.net framework)
        var loader = locations.SingleOrDefault(x => x.EndsWith("Loader.dll"));
        if (loader == null)
        {
            loader = locations.SingleOrDefault(x => x.EndsWith("Loader2.dll"));
        }
        assembliesToLoad.Add(loader);

        var unityEngine = locations.Single(x => x.EndsWith("System.dll"));
        assembliesToLoad.Add(unityEngine);
        var system = locations.Single(x => x.EndsWith("UnityEngine.dll"));
        assembliesToLoad.Add(system);
        var unityEngineCoreModule = locations.Single(x => x.EndsWith("UnityEngine.CoreModule.dll"));
        assembliesToLoad.Add(unityEngineCoreModule);
        var unityEnginePhysicsModule = locations.Single(x => x.EndsWith("UnityEngine.PhysicsModule.dll"));
        assembliesToLoad.Add(unityEnginePhysicsModule);
        var unityEngineInputLegacyModule = locations.Single(x => x.EndsWith("UnityEngine.InputLegacyModule.dll"));
        assembliesToLoad.Add(unityEngineInputLegacyModule);
        var linqAssembly = locations.Single(x => x.EndsWith("System.Core.dll"));
        assembliesToLoad.Add(linqAssembly);

        ///Loading from bytes because that does not lock assemblies for others to use.
        ///Since we only load Loader currently, that does not seem to matter
        foreach (var item in assembliesToLoad)
        {
            domain.Load(File.ReadAllBytes(item));
        }
    }
    #endregion

    #region INITIALISE_LOADER
    private static Loader InitLoader(AppDomain domain)
    {
        //domain.Load(typeof(Loader).Assembly.FullName);

        Loader loader = (Loader)Activator.CreateInstance(
            domain,
            typeof(Loader).Assembly.FullName,
            typeof(Loader).FullName,
            false,
            BindingFlags.Public | BindingFlags.Instance,
            null,
            null,
            null,
            null).Unwrap();

        return loader;
    }
    #endregion 

    #endregion

    #region LOAD_ASSEMBLY_MAIN_DOMAIN
    /// <summary>
    /// Loads an assembly from path and name or full path.
    /// Since it loads it in the main domain, that introduces memory leak. 
    /// </summary>
    public static Assembly LoadAssemblyMainDomain(string path, string name, string incFullPath = null)
    {
        var timer = new diagnostics.Stopwatch();
        var fullPath = "";
        if (incFullPath != null)
        {
            fullPath = incFullPath;
        }
        else
        {
            fullPath = $"{path}/{name}.dll";
        }

        timer.Start();
        var ass = Assembly.Load(File.ReadAllBytes(fullPath));
        timer.Stop();
        Debug.Log("Imported DLL time: " + timer.ElapsedMilliseconds);
        return ass;
    }

    private static Assembly NoMemoryLeakLoadAssembly(string path, string name, string incFullPath = null)
    {
        var fullPath = "";
        if (incFullPath != null)
        {
            fullPath = incFullPath;
        }
        else
        {
            fullPath = $"{path}/{name}.dll";
        }

        //AppDomain dom = AppDomain.CreateDomain(runtimeAppDomainName);
        AssemblyName assemblyName = new AssemblyName();
        assemblyName.CodeBase = fullPath;
        Assembly assembly = testDomain.Load(assemblyName);

        return assembly;
        //AppDomain.Unload(dom);
    }
    #endregion

    #region  SAVE_ASSEMBLY_TO_DISK
    /// <summary>
    /// Writes in memory assembly to disk.
    /// </summary>
    public static void WriteAssemblyToFile(Assembly assembly, string path, string name) /// Works
    {
        var timer = new diagnostics.Stopwatch();
        timer.Start();

        AssemblyName asmName = assembly.GetName();
        AssemblyBuilder asmBuilder = Thread.GetDomain().DefineDynamicAssembly(
            asmName, AssemblyBuilderAccess.RunAndSave, path);

        asmBuilder.Save($"{name}.dll");

        timer.Stop();
        Debug.Log("Writing Assembly Took: " + timer.ElapsedMilliseconds);
    }
    #endregion

    #region DELETE_ASSEMBLY
    /// <summary>
    /// Deletes Specified assembly file.
    /// </summary>
    public static void DeleteAssembly(string path, string name) //works
    {
        var fullName = $"{path}/{name}.dll";
        try
        {
            File.Delete(fullName);
        }
        catch (Exception e)
        {
            Debug.Log("FileNotDeleted: " + e.Message);
        }
    }
    #endregion

    #region GENERATE_ASSEMBLY

    #region GENERATE_ASSEMBLY_IN_MEMORY
    /// <summary>
    /// Generates assembly for source text and returns in-memory assembly.
    /// Since it is in-memory the assembly belongs to the main app-domain -> 
    /// which leads to memory leak
    /// </summary>
    public static Assembly GenerateAssemblyInMemory(string str, bool isPath)
    {
        Debug.Log("GenAssDefault");

        var source = "";
        if (isPath)
        {
            source = File.ReadAllText(str);
        }
        else
        {
            source = str;
        }

        var providerOptions = new Dictionary<string, string>();
        providerOptions.Add("CompilerVersion", "v4.0");

        var provider = new CSharpCodeProvider(providerOptions);

        var compilerParameters = new CompilerParameters()
        {
            GenerateExecutable = false,
            GenerateInMemory = true,
        };

        //AddReferencesToCompilerParameters(compilerParameters);

        ///Add all assemblies form Current Domain?
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            compilerParameters.ReferencedAssemblies.Add(assembly.Location);
        }

        var timer = new diagnostics.Stopwatch();
        timer.Start();
        /// You can pass multiple sources
        var compilerResult = provider.CompileAssemblyFromSource(compilerParameters, source);
        timer.Stop();
        Debug.Log("Compilation Time: " + timer.ElapsedMilliseconds);

        /// you'll need to look at result.Errors to see if it compiled, otherwise..
        foreach (var error in compilerResult.Errors)
        {
            Debug.Log("Compilation Error: " + error);
        }

        ///I beleve it returns null if compilation had Error (not Warning)
        var compiledAssembly = compilerResult.CompiledAssembly;

        return compiledAssembly;
    }
    #endregion

    #region GENERATE_ASSEMBLY_TO_FILE
    /// <summary>
    /// Generates assembly as a file and returns the location of said file. No memory leak here.
    /// </summary>
    public static string GenerateAssemblyToFile(string source)
    {
        Debug.Log("GenAssNewDomain");

        var assName = Guid.NewGuid().ToString();
        var provider = new CSharpCodeProvider();
        var compilerParameters = new CompilerParameters()
        {
            GenerateExecutable = false,
            GenerateInMemory = false,
            OutputAssembly = $"C:/Users/ASUS G751JY/source/repos/11111 Unity/RunTime/Src/Assets/Assemblies/{assName}.dll"
        };

        ///Add all assemblies form Current Domain?
        //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //{
        //    compilerParameters.ReferencedAssemblies.Add(assembly.Location);
        //}
        ///... 

        ///Add only selected DLLS
        AddReferencesToCompilerParameters(compilerParameters);

        var timer = new diagnostics.Stopwatch();
        timer.Start();
        /// You can pass multiple sources
        var compilerResult = provider.CompileAssemblyFromSource(compilerParameters, source);
        timer.Stop();
        Debug.Log("Compilation Time Domain: " + timer.ElapsedMilliseconds);

        /// you'll need to look at result.Errors to see if it compiled, otherwise..
        foreach (var error in compilerResult.Errors)
        {
            Debug.Log("Compilation Error Domain: " + error);
        }

        if (File.Exists(compilerResult.PathToAssembly))
        {
            // Debug.Log("File Exists");
        }
        else
        {
            Debug.Log("Path Not Found: " + compilerResult.PathToAssembly);
        }

        //var ass = NoMemoryLeakLoadAssembly("","",compilerResult.PathToAssembly);
        //var ass1 = LoadAssembly("","", compilerResult.PathToAssembly);

        //var bytes = File.ReadAllBytes(compilerResult.PathToAssembly);
        //var ass = sandbox.Load(bytes);

        //File.Delete(compilerResult.PathToAssembly);

        return compilerResult.PathToAssembly;
    }
    #endregion

    #region ADD_REFERENCES_TO_COMPILER_PARAMATERS
    /// <summary>
    /// Adds the references that are required by the source to comple.
    /// </summary>
    /// <param name="compilerParameters"></param>
    private static void AddReferencesToCompilerParameters(CompilerParameters compilerParameters)
    {
        ///Manualy Provide available assemblies
        var locations = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.Location);

        var allowedAsseblyReferences = new List<string>();
        var unityEngine = locations.Single(x => x.EndsWith("System.dll"));
        allowedAsseblyReferences.Add(unityEngine);
        var system = locations.Single(x => x.EndsWith("UnityEngine.dll"));
        allowedAsseblyReferences.Add(system);
        var unityEngineCoreModule = locations.Single(x => x.EndsWith("UnityEngine.CoreModule.dll"));
        allowedAsseblyReferences.Add(unityEngineCoreModule);
        var unityEnginePhysicsModule = locations.Single(x => x.EndsWith("UnityEngine.PhysicsModule.dll"));
        allowedAsseblyReferences.Add(unityEnginePhysicsModule);
        var unityEngineInputLegacyModule = locations.Single(x => x.EndsWith("UnityEngine.InputLegacyModule.dll"));
        allowedAsseblyReferences.Add(unityEngineInputLegacyModule);
        var linqAssembly = locations.Single(x => x.EndsWith("System.Core.dll"));
        allowedAsseblyReferences.Add(linqAssembly);

        var loader = locations.SingleOrDefault(x => x.EndsWith("Loader.dll")); ///TODO: Why Am I Adding it
        if (loader == null)
        {
            loader = locations.Single(x => x.EndsWith("Loader2.dll"));
        }

        allowedAsseblyReferences.Add(loader);
        var netstandard = locations.Single(x => x.EndsWith("netstandard.dll"));
        allowedAsseblyReferences.Add(netstandard);

        foreach (var item in allowedAsseblyReferences)
        {
            compilerParameters.ReferencedAssemblies.Add(item);
        }
    }
    #endregion

    #endregion

    #region GENERATE_METHODS_FROM_ASSEMBLY
    /// <summary>
    /// Gets as assembly, takes the first type and generates all methods with their paramater 
    /// types and returns the information as <see cref=CompMethodsInAssemblyType>
    /// </summary>
    public static CompMethodsInAssemblyType GenerateAllMethodsFromAssembly(Assembly ass)
    {
        var type = GetSingleTypeFromAssembly(ass);
        return GenerateAllMethodsFromMonoType(type);
    }

    /// <summary>
    /// Returns the first type from a given assembly.
    /// It is assumed for now that an assembly conteins exactly one type.
    /// </summary>
    public static Type GetSingleTypeFromAssembly(Assembly ass)
    {
        var types = ass.GetTypes();

        if (types.Length == 0)
        {
            Debug.Log("No types found in assembly!");
            return null;
        }

        ///Will be posible to extract more that one in the future
        ///TODO: Uncomment that
        //if (types.Length > 1)
        //{
        //    Debug.Log("More than one type found in assembly!");
        //    return null;
        //}

        var type = types[0];

        return type;
    }

    /// <summary>
    /// Recives a type and extracts the type name,
    /// attach function - function that recives game object and returns monobehaviour,
    /// as well as all methods with paramater types <see cref=CompMethodsInAssemblyType>
    /// </summary>
    public static CompMethodsInAssemblyType GenerateAllMethodsFromMonoType(Type type)
    {
        var result = new CompMethodsInAssemblyType();

        ///Getting the attach method and converting it to Funk that attaches to GameObject and returns the MonoBehaviour
        var attachMethod = type.GetMethod("Attach");

        ///if no attachMethod throw
        if (attachMethod == null)
        {
            Debug.Log("There is not attach method on Type: " + type.Name);
            ///TODO: I also use this to get methods for compile time types that do not need attaching 
            ///separate both use cases?
            //throw new Exception("There is not attach method on Type: " + type.Name);
        }
        else
        {
            ///Creating the attach function if we find Attach method in the type
            ///The attach method is used to attach the monobehaviour to GameObject and return the attached Instance
            var attachFunk = (Func<GameObject, MonoBehaviour>)
                Delegate.CreateDelegate(typeof(Func<GameObject, MonoBehaviour>), attachMethod);
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
        foreach (var methodInfo in otherMethods)
        {
            var parameterInfos = methodInfo.GetParameters();
            var parameters = new List<UiParameterWithType>();

            foreach (var parInfo in parameterInfos)
            {
                parameters.Add(new UiParameterWithType
                {
                    Name = parInfo.Name,
                    Type = parInfo.ParameterType
                });
            }

            methodInfos[methodInfo.Name] = new CompMethodInfoWIthParams
            {
                MethodInfo = methodInfo,
                Parameters = parameters.ToArray(),
            };
        }
        ///...

        result.MethodInfos = methodInfos;
        result.TypeName = type.Name;

        return result;
    }
    #endregion

    #region ADD_SELF_ATTACH_TO_SOURCE
    /// <summary>
    /// Finds the closing bracket and just above it insert the Attach method, so 
    /// it does not have to be pre added to every source the user submits 
    /// </summary>
    public static string AddSelfAttachToSource(string source)
    {
        /// spliting the code by lines and trims then as well as remove empty lines,
        /// it is assumed that brackets {,} will awlays be on new lines
        var lines = GetTrimedSourceLines(source).ToList();

        ///If last line is not soly } after trim return null
        if (lines[lines.Count - 1] != "}")
        {
            Debug.LogError("File does not end with \"}\"");
            return null;
        }

        ///If can not find opening bracket return null
        var openingBraketLineNumber = lines.IndexOf("{");
        if (openingBraketLineNumber == -1)
        {
            Debug.Log("Can not find opening bracket braket!");
            return null;
        }

        ///The line right above is expected to be the class declaration line, 
        ///from which we can extract the name of the class
        var classLine = lines[openingBraketLineNumber - 1];

        ///we get the parts of the class declaration line
        var classDeclarationLineParts = classLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        ///we find the index of class inside the above class declaration line parts
        var indexOfClass = Array.IndexOf(classDeclarationLineParts, "class");

        ///if there is no class keyword in this line we return null
        if (indexOfClass == -1)
        {
            Debug.Log("Can not find class keyword in the class declaration line!");
            return null;
        }

        ///if the class keyword is the last item in the parts array, return null since we can not get name
        if (classDeclarationLineParts.Length - 1 == indexOfClass)
        {
            Debug.Log("Can not find the name of the class after the class keyword!");
            return null;
        }

        var name = string.Empty;
        ///we get tha part just after class, as it should contein the class name
        var namePart = classDeclarationLineParts[indexOfClass + 1];
        ///if that contains :, which is valid inheratance syntaxis, we take just the part before : which is the name
        if (namePart.Contains(":"))
        {
            name = namePart.Substring(0, namePart.IndexOf(":"));
        }
        ///if not, we just take the whole thing as the name
        else
        {
            name = namePart;
        }

        if (name.Length == 0)
        {
            Debug.Log("Name not found!");
            return null;
        }

        /// we insert that bit of text at the end of the class so we can later use reflection on that fucntion 
        /// and use to attach the whole class to gameobject and return the instance of the class that is attached
        var toInsrt = $@"//--ATTACH--METHOD--HERE
public static {name} Attach(GameObject obj)
{{
    return obj.AddComponent<{name}>();
}}";

        string[] classDecLines = lines.Where(x => x.StartsWith("public class") || x.StartsWith("private class") || x.StartsWith("class")).ToArray();

        if (classDecLines.Length == 1)
        {
            lines.Insert(lines.Count - 1, toInsrt);
        }
        else
        {
            int ind = Array.IndexOf(lines.ToArray(), classDecLines[1]) -1;
            lines.Insert(ind, toInsrt);
        }

        ///joining the whole text together
        var result = string.Join("\n", lines);

        return result;
    }
    #endregion

    #region GET_TRIMED_SOURCE_LINES
    /// <summary>
    /// Trims source, removes empty lines and removes comments
    /// </summary>
    public static List<string> GetTrimedSourceLines(string source)
    {
        ///split by new line and trim all white spaces
        var lines = source
            .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim(' ', '\t', '\r', '\t', '\v'))
            .ToList();

        /// removing all empty lines
        lines = lines.Where(x => x != "").ToList();

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
        ///
        #region REMOVE /**/ COMMENTS
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
            ///Comment brackets mismatch///Questionable if necessary
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
                var closingBracket = closingBrackets.Where(x => x.CompareTo(openingBraket) > 0).FirstOrDefault();

                if (closingBracket == null)
                {
                    Debug.Log("Braket mismath");
                    return null;
                }

                ///delete the whole comented lines
                if (closingBracket.Line - openingBraket.Line > 1)
                {
                    for (int i = openingBraket.Line + 1; i < closingBracket.Line; i++)
                    {
                        lines[i] = "";
                    }
                }
                ///...

                /*Delete the comment from partially commented lines*/
                //Debug.Log(openingBraket + "|" + closingBracket);
                //Debug.Log(closingBracket.Col + 2 + " " + lines[closingBracket.Line].Length);

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
        #endregion

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
            var lineComp = this.Line.CompareTo(other.Line);
            if (lineComp == 0)
            {
                return this.Col.CompareTo(other.Col);
            }

            return lineComp;
        }

        public override string ToString()
        {
            return $"Line: {this.Line} Col: {this.Col}";
        }
    }
    #endregion

    #region GET_ALL_CS_FILES
    /// <summary>
    /// Gets all cs files at a given path(the place where the user writes their code)
    /// </summary>
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
    #endregion

    #region TESTS

    #region LOAD_SEPERATE_ASSEMBLY_TESTS_FAIL
    /// <summary>
    /// Tried to find a way of attaching monos generated in a separate assemblie but 
    /// there does not seem to be a way.
    /// </summary>
    //#region LOAD_ASSEMBLIES_TO_DOMAIN_X
    //public static void AddCSharpAssembliesToDomain(AppDomain domain)
    //{
    //    var locations = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.Location);
    //    var path = locations.Single(x => x.EndsWith("Assembly-CSharp.dll"));
    //    var bytes = File.ReadAllBytes(path);
    //    domain.Load(bytes);
    //}

    //public static void AddAllCurrentDomainAssembliesToDomain(AppDomain domain)
    //{
    //    foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
    //    {
    //        domain.Load(File.ReadAllBytes(ass.Location));
    //    }
    //}
    //#endregion

    //#region DOMAIN_SET_UPS
    //private static AppDomain SetUpAppDomainFull(string name)
    //{
    //    AppDomainSetup ads = new AppDomainSetup();
    //    ads.ApplicationName = "RunTime";
    //    ads.ApplicationBase = "C:/Users/ASUS G751JY/source/repos/11111 Unity/RunTime/Src/Assets/Assemblies";
    //    ads.DynamicBase = "C:/Users/ASUS G751JY/source/repos/11111 Unity/RunTime/Src/Assets/Assemblies";
    //    ads.PrivateBinPath = "C:/Users/ASUS G751JY/source/repos/11111 Unity/RunTime/Src/Assets/Assemblies";
    //    ads.ShadowCopyFiles = "true";
    //    ads.DisallowBindingRedirects = false;
    //    ads.DisallowCodeDownload = true;
    //    //ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
    //    ads.DisallowApplicationBaseProbing = true; //X if true AssemblyResolve stays in the domain else goes to the main domain

    //    var evidence = AppDomain.CurrentDomain.Evidence;

    //    var domain = AppDomain.CreateDomain(name, evidence, ads);
    //    Debug.Log("DOMAIN_NAME_FULL: " + domain.FriendlyName);
    //    return domain;
    //}

    //private static AppDomain SetUpAppDomainMinimal(string name)
    //{
    //    var domain = AppDomain.CreateDomain(name);
    //    Debug.Log("DOMAIN_NAME_MINIMAL: " + domain.FriendlyName);
    //    return domain;
    //}

    //private static AppDomain SetUpAsMainWithLoaderOptimization(string name, LoaderOptimization optimization)
    //{
    //    var info = AppDomain.CurrentDomain.SetupInformation;
    //    info.LoaderOptimization = optimization;

    //    var domain = AppDomain.CreateDomain(
    //        name,
    //        AppDomain.CurrentDomain.Evidence,
    //        info);

    //    return domain;
    //}
    //#endregion
    //public static void GetTypesFromAssemblySeparateDomain(string assemblyFullName)
    //{
    //    //FullTest(assemblyFullName);
    //    //DirectLoadTest(assemblyFullName);
    //}
    //private static void DirectLoadTest(string assemblyFullName)
    //{
    //    testDomain = SetUpAsMain("testy");
    //    AddAllCurrentDomainAssembliesToDomain(testDomain);
    //    var name = AssemblyName.GetAssemblyName(assemblyFullName).FullName;
    //    //AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assemblyFullName));
    //    testDomain.Load(AssemblyName.GetAssemblyName(assemblyFullName));
    //}

    //private static void FullTest(string assemblyFullName)
    //{
    //    testDomain = SetUpAsMain("testy");
    //    AddAllCurrentDomainAssembliesToDomain(testDomain);
    //    var loader = InitLoader(testDomain);
    //    UseLoader(testDomain, assemblyFullName, loader);
    //    FinalTest(assemblyFullName);
    //}
    //private static void UseLoader(AppDomain domain, string assemblyFullName, Loader loader)
    //{
    //    var bytes = File.ReadAllBytes(assemblyFullName);
    //    loader.FinalAttempt(bytes);
    //    Debug.Log("After Deserialization");
    //}
    ///.............................................................................................
    //public static void UnloadCurrentAppDomain()
    //{
    //    AppDomain.Unload(AppDomain.CurrentDomain); /// Does not work
    //}

    /// Do not remember that working
    //#region ONE_CLICK_TEST
    //public static void AttachMonoFromSeparateDomain()
    //{
    //    testDomain = SetUpAsMain("testy");
    //    AddNecessaryAssembliesToDomain(testDomain);
    //    //testDomain.AssemblyResolve += AssemblyResolveSeparateDomain;
    //    //AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveMainDomain;

    //    var main = GameObject.Find("Main");
    //    main.AddComponent<LoaderAssemblyMonoTest>();

    //    var loader = InitLoader(testDomain);
    //    loader.GetComponentFromAttachedObject();
    //}
    //#endregion
    #endregion

    #region TEST_IF_ASSEMBLY_IS_ONLY_LOADED_IN_SEPARATE_DOMAIN
    public static void TestOnlyLoadedInRightAssembly(string assemblyFullName) ///Bad method, forced a the assemblie to be loaded Again 
    {
        /// testing that the assembly is only loaded in one place
        var tokens = assemblyFullName.Split(new char[] { '/', '\\', '.' });
        var name = tokens[tokens.Length - 2];

        var mainHasIt = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => x.FullName)
            .Any(x => x.Contains(name));

        var testHasIt = testDomain
            .GetAssemblies()
            .Select(x => x.FullName)
            .Any(x => x.Contains(name));

        var mainHasItAfter = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => x.FullName)
            .Any(x => x.Contains(name));

        ///false true true Is successul result 
        Debug.Log("main " + mainHasIt + " test " + testHasIt + " main after " + mainHasItAfter);
        ///...
    }
    #endregion

    #endregion

    #region EXPERIMENTAL
    #region FREE_LIBRARY_TESTS

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string libname);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool FreeLibrary(IntPtr hModule);

    /// <summary>
    /// Loads the library, however crashes spectacularly if you try access said library again
    /// </summary>
    public static void FreeLibraryTest()
    {
        var assembly = Assembly.Load(File.ReadAllBytes(testAssemblyFullName));

        TestDll(assembly);

        IntPtr myLibrary = IntPtr.Zero;

        foreach (sd.ProcessModule item in sd.Process.GetCurrentProcess().Modules)
        {
            if (item.FileName.Contains(testAssemblyName))
            {
                myLibrary = item.BaseAddress;
                break;
            }
        }

        if (myLibrary != IntPtr.Zero)
        {
            FreeLibrary(myLibrary);
            Debug.Log("released the library");
        }
        else
        {
            Debug.Log("process not found");
        }

        ///It doed free the library but
        ///if I remember correctly Unity crashes if you try to access assembly that was FreeLibrary-ed
        //AppDomain.CurrentDomain.GetAssemblies();
        //TestDll(assembly);
    }

    public static void TestDll(Assembly ass)
    {
        var type = ass.GetTypes()[0];
        Debug.Log(type.Name + "Works!");
    }

    public static void MemoryTest()
    {
        var bytes = File.ReadAllBytes(testAssemblyFullName);

        for (int i = 0; i < 100; i++)
        {
            Assembly.Load(bytes);
        }

        Debug.Log("Loaded 100 assemblies");
    }

    #endregion
    #endregion

    #region DEPRICATED 
    #region GET_DIFFERENCE 
    //private static void GetDifferences(IEnumerable<string> more, IEnumerable<string> less, string name)
    //{
    //    var diff = new List<string>();
    //    foreach (var item in more)
    //    {
    //        if (!less.Contains(item))
    //        {
    //            diff.Add(item);
    //        }
    //    }

    //    if (diff.Count == 0)
    //    {
    //        Debug.Log(name + " No Differences");
    //    }
    //    else
    //    {
    //        Debug.Log(name + " Differences Here &&&&&&&&&&&&&&");
    //        foreach (var item in diff)
    //        {
    //            Debug.Log(item);
    //        }
    //        Debug.Log(name + " EndDifferences Here &&&&&&&&&&&");
    //    }

    //    var diff2 = new List<string>();
    //    foreach (var item in less)
    //    {
    //        if (!more.Contains(item))
    //        {
    //            Debug.Log("Less Has Items More Does Not &&&&&&&&&&");
    //        }
    //    }
    //}
    #endregion
    #region UNHANDLED_EXCEPTIONS
    //public static void UnhandledExceptionHandlerMain(object obj, UnhandledExceptionEventArgs args)
    //{
    //    Debug.Log("MAIN_UNHADLED_EXCEPTION############");
    //    Debug.Log(args.ToString());
    //    Debug.Log("MAIN_UNHADLED_EXCEPTION_END########");
    //}

    //public static void UnhandledExceptionHandlerSeparate(object obj, UnhandledExceptionEventArgs args)
    //{
    //    Debug.Log("SEPARATE_UNHADLED_EXCEPTION############");
    //    Debug.Log(args.ToString());
    //    Debug.Log("SEPARATE_UNHADLED_EXCEPTION_END########");
    //}
    #endregion
    #region ASSEMBLY_RESOLVES
    //private static Assembly AssemblyResolveMainDomain(object sender, ResolveEventArgs args)
    //{
    //    Debug.Log("MAIN_RESOLVER#################################");
    //    //Debug.Log("requesting assembly " + args.RequestingAssembly.FullName); ///Serialization exception
    //    var mainDomain = (AppDomain)sender;
    //    Debug.Log("UCD Assemblie Count => " + mainDomain.GetAssemblies().Length);
    //    //Debug.Log("testy Assemblie Count => " + userDomain.GetAssemblies().Length); ///Can not load the current Assembly
    //    Debug.Log("IsDefault: " + mainDomain.IsDefaultAppDomain());

    //    //var setUp = domain.SetupInformation;
    //    Debug.Log("Domain Name " + mainDomain.FriendlyName); //Unity Child Domain
    //    Debug.Log("searched for: " + args.Name);
    //    var name = args.Name.Split(',').First();
    //    var fullPath = $"{RunTimeAssembliesPath}/{name}.dll";
    //    Debug.Log("FULL_NAME_RECUNSTRUCTED => " + fullPath);
    //    var bytes = File.ReadAllBytes(fullPath);
    //    Debug.Log(bytes.Length);
    //    Debug.Log("userDomainNam: " + testDomain.FriendlyName);
    //    var ass = Assembly.Load(bytes);
    //    Debug.Log("MAIN_RESOLVER_END#############################");
    //    return ass;
    //}

    private static Assembly AssemblyResolveSeparateDomain(object sender, ResolveEventArgs args)
    {
        Debug.Log("SEPARATE_RESOLVER#################################");
        var childDomain = (AppDomain)sender;
        Debug.Log("Domain Name " + childDomain.FriendlyName);
        Debug.Log("searched for: " + args.Name);
        var name = args.Name.Split(',').First();
        var dllName = $"{name}.dll";
        var fullPath = $"{RunTimeAssembliesPath}/{dllName}";
        Debug.Log("FULL_NAME_RECUNSTRUCTED => " + fullPath);

        //var assPath = AppDomain.CurrentDomain.GetAssemblies()
        //    .Select(x => x.Location)
        //    .Where(x => x.EndsWith(dllName))
        //    .SingleOrDefault();

        //if (assPath != null)
        //{
        //    Debug.Log("Returned Assembly From The Main: " + assPath);
        //    Debug.Log("SEPARATE_RESOLVER_END#############################");
        //    return Assembly.LoadFile(assPath);
        //}

        //if (name == "Loader")
        //{
        //    Debug.Log("Reterned Loader");
        //    return Assembly.Load(typeof(Loader).Assembly.FullName);
        //}

        //var bytes = File.ReadAllBytes(fullPath);
        //Debug.Log(bytes.Length);
        //var ass = childDomain.Load(bytes);
        //Debug.Log("Loaded Ass Full Name " + ass.FullName);
        //Debug.Log("SEPARATE_RESOLVER_END#############################");
        return null;
    }
    #endregion
    #region GENERATE_SOLVE_METHOD
    //public static CompTypeWithSolveMehtodInfo GenerateTypeWithSolveMethod(Assembly ass)
    //{
    //    var type = GetSingleTypeFromAssembly(ass);
    //    var solveMehtod = type.GetMethod("Solve");
    //    if (solveMehtod == null)
    //    {
    //        Debug.Log("Solve Method Not Found!");
    //        throw new Exception("Solve Method Not Found!");
    //    }

    //    var result = new CompTypeWithSolveMehtodInfo
    //    {
    //        ClassType = type,
    //        SolveMethodInfo = solveMehtod,
    //    };

    //    return result;
    //}
    #endregion
    #endregion

    #region END_BRACKET
}
#endregion
