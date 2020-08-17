using System;
using UnityEngine;

public class MethodNode : MonoBehaviour
{
    private WorldSpaceUI UI;
    private Type type;
    public object Object { get; set; }
    public MyMethodInfo MyMethodInfo { get; set; }
    public MyParameterInfo[] MyParamaters { get; set; }
    public int ID { get; set; }
    public Node ClassNode { get; set; }
    
    public void Setup(MyMethodInfo methodInfo, MyParameterInfo[] myParamaters, object classObject, WorldSpaceUI UI, Node classNode)
    {
        this.MyMethodInfo = methodInfo;
        this.MyParamaters = myParamaters;
        this.Object = classObject;
        this.UI = UI;
        this.ID = methodInfo.ID;
        this.type = methodInfo.Info.ReturnType;
        this.ClassNode = classNode;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.MyMethodInfo.Info.Name}, Type: {this.type.Name}";
        this.UI.infoCanvas.SetTextWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        this.UI.infoCanvas.SetTextWorldCanvasText(message);
        this.UI.connRegisterer.RegisterMethodClick(this);
    }
}