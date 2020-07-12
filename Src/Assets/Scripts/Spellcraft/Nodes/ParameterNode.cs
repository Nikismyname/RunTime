using System.Reflection;
using UnityEngine;

public class ParameterNode :MonoBehaviour
{
    private WorldSpaceUI UI;
    public object Object { get; set; }
    public ParameterInfo ParameterInfo { get; set; }

    private bool assigned = false;

    public void Setup(ParameterInfo PropertyInfo, object ClassObject,WorldSpaceUI UI)
    {
        this.ParameterInfo = PropertyInfo; 
        this.Object = ClassObject;
      
        this.UI = UI;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.ParameterInfo.Name}, Type: {this.ParameterInfo.ParameterType.Name}";
        this.UI.SetTextWorldCanvasPosition(this.gameObject.transform.parent.transform.position+ new Vector3(0,1,0));
        this.UI.SetTextWorldCanvasText(message);
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
        this.gameObject.SetColor(Color.blue);
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

