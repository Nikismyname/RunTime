using System.Reflection;
using UnityEngine;

public class PropertyNode : MonoBehaviour
{
    private InGameUI UI;
    public object ClassObject { get; set; }
    PropertyInfo PropertyInfo { get; set; }

    public void Setup(PropertyInfo propertyInfo, object classObject, InGameUI UI)
    {
        this.PropertyInfo = propertyInfo;
        this.ClassObject = classObject;

        this.UI = UI;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.PropertyInfo.Name}, Type: {this.PropertyInfo.PropertyType.Name}";
        this.UI.SetWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        this.UI.SetWorldCanvasText(message);
        this.UI.RegisterPropertyClick(this);
    }
}

