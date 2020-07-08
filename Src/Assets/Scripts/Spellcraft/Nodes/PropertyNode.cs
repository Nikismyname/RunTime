using System;
using UnityEngine;

public class PropertyNode :MonoBehaviour
{
    private string name;
    private Type type;
    private InGameUI UI;

    public void Setup(string name, Type type, InGameUI UI)
    {
        this.name = name;
        this.type = type;
        this.UI = UI;
    }

    private void OnMouseDown()
    {
        string message = $"Name: {this.name}, Type: {this.type.Name}";
        this.UI.SetWorldCanvasPosition(this.gameObject.transform.parent.transform.position+ new Vector3(0,1,0));
        this.UI.SetWorldCanvasText(message);
        Debug.Log("HERE");
    }
}

