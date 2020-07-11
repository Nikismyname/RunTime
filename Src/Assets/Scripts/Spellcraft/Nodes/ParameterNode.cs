using System.Reflection;
using UnityEngine;

public class ParameterNode :MonoBehaviour
{
    private InGameUI UI;
    public object Object { get; set; }
    public ParameterInfo ParameterInfo { get; set; }

    private bool assigned = false;

    public void Setup(ParameterInfo PropertyInfo, object ClassObject,InGameUI UI)
    {
        this.ParameterInfo = PropertyInfo; 
        this.Object = ClassObject;
      
        this.UI = UI;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.ParameterInfo.Name}, Type: {this.ParameterInfo.ParameterType.Name}";
        this.UI.SetWorldCanvasPosition(this.gameObject.transform.parent.transform.position+ new Vector3(0,1,0));
        this.UI.SetWorldCanvasText(message);
        this.UI.RegisterParameterClick(this);
    }

    public void RegisterSelection()
    {
        if (assigned)
        {
            return;
        }

        this.gameObject.SetColor(Color.gray);
    }

    public void RegisterAssignment()
    {
        this.gameObject.SetColor(Color.white);
        this.assigned = true;
    }

    public void RegisterDeselection()
    {
        if (this.assigned)
        {
            return; 
        }

        this.gameObject.SetColor(Color.black);
    }
}

