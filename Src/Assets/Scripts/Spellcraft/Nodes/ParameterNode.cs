using System.Reflection;
using UnityEngine;

public class ParameterNode : MonoBehaviour
{
    private WorldSpaceUI UI;
    private bool assigned = false;
    
    public object Object { get; set; }
    public MyParameterInfo ParameterInfo { get; set; }
    public MethodInfo myMethod { get; set; }
    public int ID { get; set; }
    public Node ClassNode { get; set; }
    
    public void Setup(MyParameterInfo PropertyInfo, MethodInfo myMethod, object ClassObject, WorldSpaceUI UI, Node classNode)
    {
        this.ParameterInfo = PropertyInfo;
        this.Object = ClassObject;
        this.myMethod = myMethod;
        this.ID = PropertyInfo.ID;
        this.UI = UI;
        this.ClassNode = classNode; 
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.ParameterInfo.Info.Name}, Type: {this.ParameterInfo.Info.ParameterType.Name}";
        this.UI.infoCanvas.SetTextWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        this.UI.infoCanvas.SetTextWorldCanvasText(message);
        _ = this.UI.connRegisterer.RegisterParameterClick(this);
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

