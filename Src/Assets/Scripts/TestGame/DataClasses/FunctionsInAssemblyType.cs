using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

///Compilation Data Structures

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
