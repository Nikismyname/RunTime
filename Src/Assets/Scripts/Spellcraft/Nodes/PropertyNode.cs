using System.Reflection;
using UnityEngine;

public class PropertyNode : MonoBehaviour
{
    private WorldSpaceUI UI;
    public object ClassObject { get; set; }
    PropertyInfo PropertyInfo { get; set; }

    public void Setup(PropertyInfo propertyInfo, object classObject, WorldSpaceUI UI)
    {
        this.PropertyInfo = propertyInfo;
        this.ClassObject = classObject;

        this.UI = UI;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.PropertyInfo.Name}, Type: {this.PropertyInfo.PropertyType.Name}";
        this.UI.infoCanvas.SetTextWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        this.UI.infoCanvas.SetTextWorldCanvasText(message);
        this.UI.connRegisterer.RegisterPropertyClick(this);
    }
}

