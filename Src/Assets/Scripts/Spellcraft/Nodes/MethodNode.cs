using System;
using System.Reflection;
using UnityEngine;

public class MethodNode : MonoBehaviour
{
    private InGameUI UI;
    private Type type;
    public object Object { get; set; }
    public MethodInfo MethodInfo { get; set; }
    public ParameterInfo[] MyParamaters { get; set; }

    public void Setup(MethodInfo methodInfo, ParameterInfo[] myParamaters, object classObject, InGameUI UI)
    {
        this.MethodInfo = methodInfo;
        this.MyParamaters = myParamaters;
        this.Object = classObject;
        this.UI = UI;

        this.type = methodInfo.ReturnType;
    }

    private void OnMouseDown()
    {
        string message = $"Name: Some, Type: {this.type.Name}";
        this.UI.SetWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        this.UI.SetWorldCanvasText(message);
        this.UI.RegisterMethodClick(this);
    }
}