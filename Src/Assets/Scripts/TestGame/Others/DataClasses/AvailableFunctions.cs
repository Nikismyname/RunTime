using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

///> Those Classes Store The Information In The Main
public class MainMonoWithName
{
    public MainMonoWithName() { }

    public MainMonoWithName(string name, MonoBehaviour mono)
    {
        this.Name = name;
        this.Mono = mono;
        this.MyMethods = new List<MainMethodInfoWithName>();
    }

    public string Name { get; set; }
    public MonoBehaviour Mono { get; set; }
    public int Version { get; set; }
    public bool changesInMethodSignature { get; set; }

    public List<MainMethodInfoWithName> MyMethods { get; set; }
}

public class MainMethodInfoWithName
{
    public MainMethodInfoWithName() { }

    public MainMethodInfoWithName(string name, MethodInfo methodInfo, UiParameterWithType[] parameters)
    {
        this.Name = name;
        this.MethodInfo = methodInfo;
        this.Parameters = parameters;
    }

    public string Name { get; set; }
    public MethodInfo MethodInfo { get; set; }
    public UiParameterWithType[] Parameters { get; set; }
}
///...

///> Those Clases Are Sent To The Buttons!

public class UiMonoWithMethods
{
    public GameObject Object { get; set; }
    public string MonoName { get; set; }
    public UiMethodNameWithParameters[] Methods { get; set; }
    public string Source { get; set; }
}

public class UiMethodNameWithParameters
{
    public string Name { get; set; }
    public UiParameterWithType[] Parameters { get; set; }
}

public class UiParameterWithType
{
    public string Name { get; set; }
    public Type Type { get; set; }
}

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