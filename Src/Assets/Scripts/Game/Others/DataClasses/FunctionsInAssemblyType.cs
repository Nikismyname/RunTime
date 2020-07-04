using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// Compilation Data Structures
/// Monobehaviour Compilation
public class CompMethodsInAssemblyType
{
    public string TypeName { get; set; }

    public Func<GameObject, MonoBehaviour> Attach { get; set; }

    public Dictionary<string, CompMethodInfoWIthParams> MethodInfos { get; set; }
}

public class CompMethodInfoWIthParams
{
    public MethodInfo MethodInfo { get; set; }

    public UiParameterWithType[] Parameters { get; set; }
}
///...

///Code To Be Tested Compilation
public class CompTypeWithSolveMehtodInfo
{
    public Type ClassType { get; set; }
    public MethodInfo SolveMethodInfo { get; set; }
}
