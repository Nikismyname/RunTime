using System.Collections.Generic;
using UnityEngine;

public class ConstructLevel : MonoBehaviour
{
    public List<GameObject> allObjs;

    private void Start()
    {
        this.allObjs = new List<GameObject>();

        var data = ObjectSerialising.DeserialiseObjects("","");

        foreach (var dataPint in data)
        {
            this.CreateObj(dataPint);
        }
    }

    private void CreateObj(PrimitiveObjectSerialiseData data)
    {
        var go = GameObject.CreatePrimitive(data.type);
        go.transform.position = data.position;
        go.transform.localScale = data.scale;
        go.transform.eulerAngles = data.rotation;
        go.GetComponent<Renderer>().material.color = data.color;

        var dm = go.AddComponent<PrimitiveObjectDataModifier>(); 

        this.allObjs.Add(go);
    }
}

