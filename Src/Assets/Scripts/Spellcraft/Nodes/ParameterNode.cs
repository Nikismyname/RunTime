using System;
using System.Reflection;
using UnityEngine;

public class ParameterNode :MonoBehaviour
{
    private InGameUI UI;
    public object ClassObject { get; set; }
    ParameterInfo ParameterInfo { get; set; } 

    public void Setup(ParameterInfo PropertyInfo, object ClassObject,InGameUI UI)
    {
        this.ParameterInfo = PropertyInfo; 
        this.ClassObject = ClassObject;
      
        this.UI = UI;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.ParameterInfo.Name}, Type: {this.ParameterInfo.ParameterType.Name}";
        this.UI.SetWorldCanvasPosition(this.gameObject.transform.parent.transform.position+ new Vector3(0,1,0));
        this.UI.SetWorldCanvasText(message);
        this.UI.RegisterParameterClick(this);
    }
}

