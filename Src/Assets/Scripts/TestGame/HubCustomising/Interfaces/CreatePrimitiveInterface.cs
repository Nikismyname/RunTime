using UnityEngine;

public static class CreatePrimitiveInterface
{
    public static void CreatePrimitive(PrimitiveObjectSerialiseData  data)
    {
        var obj = GameObject.CreatePrimitive(data.type);
        obj.transform.position = data.position;
        obj.transform.eulerAngles = data.rotation;
        obj.transform.localScale = data.scale;
        obj.GetComponent<Renderer>().material.color = data.color;

        if(data.type == PrimitiveType.Cylinder)
        {
            GameObject.Destroy(obj.GetComponent<CapsuleCollider>());
            var col = obj.AddComponent<MeshCollider>();
            col.convex = true; 
        }

        var script = obj.AddComponent<PrimitiveObjectDataModifier>();
        script.SetUp(data);
        obj.AddComponent<TargetBehaviour>(); 

        GameObject.Find("Main").GetComponent<Main>().RegisterTarget(obj); 
    }
}
