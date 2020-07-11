using System;
using System.Reflection;
using UnityEngine;

public class MethodNode : MonoBehaviour
{
    private InGameUI UI;

    private string myName;
    private Type type;
    public object ClassObject { get; set; }
    MethodInfo MethodInfo { get; set; }
    ParameterInfo[] MyParamaters { get; set; }

    public void Setup(MethodInfo methodInfo, ParameterInfo[] myParamaters, object classObject, InGameUI UI)
    {
        this.MethodInfo = methodInfo;
        this.MyParamaters = myParamaters;
        this.ClassObject = classObject;
        this.UI = UI;

        this.type = methodInfo.ReturnType;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.myName}, Type: {this.type.Name}";
        this.UI.SetWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        this.UI.SetWorldCanvasText(message);
        this.UI.RegisterMethodClick(this);
    }
}