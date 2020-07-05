using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Those Clases Are Sent To The TargetManager!
/// </summary>
/// TODO: Why do we store them different in the TM and ProcUI
public class TargetManagerMonoWithNameAndMethods
{
    public TargetManagerMonoWithNameAndMethods() { }

    public TargetManagerMonoWithNameAndMethods(string name, MonoBehaviour mono)
    {
        this.Name = name;
        this.Mono = mono;
        this.Methods = new List<TargetManagerMethodInfoWithName>();
    }

    public string Name { get; set; }
    public MonoBehaviour Mono { get; set; }
    public int Version { get; set; }
    public bool changesInMethodSignature { get; set; }

    public List<TargetManagerMethodInfoWithName> Methods { get; set; }
}

/// <summary>
/// /// Those Clases Are Sent To The TargetManager!
/// </summary>
public class TargetManagerMethodInfoWithName
{
    public TargetManagerMethodInfoWithName() { }

    public TargetManagerMethodInfoWithName(string name, MethodInfo methodInfo, UiParameterWithType[] parameters)
    {
        this.Name = name;
        this.MethodInfo = methodInfo;
        this.Parameters = parameters;
    }

    public string Name { get; set; }
    public MethodInfo MethodInfo { get; set; }
    public UiParameterWithType[] Parameters { get; set; }
}




/// <summary>
/// Those Clases Are Sent To The ProcUI!
/// </summary>
public class UiMonoWithMethods
{
    public GameObject Object { get; set; }
    public string MonoName { get; set; }
    public UiMethodNameWithParameters[] Methods { get; set; }
    public string Source { get; set; }
}

/// <summary>
/// Those Clases Are Sent To The ProcUI!
/// </summary>
public class UiMethodNameWithParameters
{
    public string Name { get; set; }
    public UiParameterWithType[] Parameters { get; set; }
}

/// <summary>
/// Those Clases Are Sent To The ProcUI!
/// </summary>
public class UiParameterWithType
{
    public string Name { get; set; }
    public Type Type { get; set; }
}

/// <summary>
/// Those Clases Are Sent To The ProcUI!
/// </summary>
public class UiMonoGroupInformation
{
    public string MonoName { get; set; }
    public GameObject GrandParent { get; set; }
    public GameObject Methods { get; set; }
    public GameObject MonoButtonLabel { get; set; }

    public bool Collapsed { get; set; }
    public float CollapsedHeight { get; set; }
    public float WholeHeight { get; set; }
    public string  Source { get; set; }
}
///...