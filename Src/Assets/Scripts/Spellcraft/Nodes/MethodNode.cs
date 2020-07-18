using System;
using System.Reflection;
using UnityEngine;

public class MethodNode : MonoBehaviour
{
    private WorldSpaceUI UI;
    private Type type;
    public object Object { get; set; }
    public MethodInfo MethodInfo { get; set; }
    public MyParameterInfo[] MyParamaters { get; set; }

    public void Setup(MethodInfo methodInfo, MyParameterInfo[] myParamaters, object classObject, WorldSpaceUI UI)
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
        this.UI.infoCanvas.SetTextWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        this.UI.infoCanvas.SetTextWorldCanvasText(message);
        this.UI.connRegisterer.RegisterMethodClick(this);
    }
}