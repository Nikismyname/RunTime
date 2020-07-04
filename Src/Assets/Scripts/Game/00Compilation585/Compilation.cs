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

    #region }
}
#endregion
