using System;
using UnityEngine;

public class ResultNode : MonoBehaviour
{
    private WorldSpaceUI UI;
    Type type; 

    public void Setup(Type type , WorldSpaceUI UI)
    {
        this.type = type; 
        this.UI = UI;
    }

    private void OnMouseDown()
    {
        //string message = $"Name: The RESULT, Type: {this.type.Name}";
        //this.UI.SetWorldCanvasPosition(this.gameObject.transform.parent.transform.position + new Vector3(0, 1, 0));
        //this.UI.SetWorldCanvasText(message);
        this.UI.RegisterResultClick(this);
    }
}

