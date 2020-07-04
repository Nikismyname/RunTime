using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using sd = System.Diagnostics;

public class ExperimentalCompilation
{
    public const string BasePath = "C:/Users/ASUS G751JY/source/repos/11111 Unity/UserCode";
    public const string RunTimeAssembliesPath = "C:/Users/ASUS G751JY/source/repos/11111 Unity/RunTime/Src/Assets/Assemblies";
    public const string NewAssembliesLocation = "C:/Users/ASUS G751JY/source/repos/11111 Unity/Assembies";
    public const string testAssemblyFullName = "C:/Users/ASUS G751JY/source/repos/11111 Unity/Assembies/8b15ac31-e824-4ee5-b2a9-3940498e5709.dll";
    public const string testAssemblyName = "8b15ac31-e824-4ee5-b2a9-3940498e5709";

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
}

