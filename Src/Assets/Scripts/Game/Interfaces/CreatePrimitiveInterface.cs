using UnityEngine;

public static class CreatePrimitiveInterface
{
    private static int counter = 0; 

    public static void CreatePrimitive(PrimitiveObjectSerialiseData  data)
    {
        var createdPrimitive = GameObject.CreatePrimitive(data.type);
        createdPrimitive.name = "CreatedPrimitive " + counter++;
        createdPrimitive.transform.position = data.position;
        createdPrimitive.transform.eulerAngles = data.rotation;
        createdPrimitive.transform.localScale = data.scale;
        createdPrimitive.GetComponent<Renderer>().material.color = data.color;

        if(data.type == PrimitiveType.Cylinder)
        {
            GameObject.Destroy(createdPrimitive.GetComponent<CapsuleCollider>());
            var col = createdPrimitive.AddComponent<MeshCollider>();
            col.convex = true; 
        }

        var dataModifierScript = createdPrimitive.AddComponent<PrimitiveObjectDataModifier>();
        dataModifierScript.SetUp(data);

        var ms = GameObject.Find("Main").GetComponent<TargetManagerBehaviour>();

        createdPrimitive.AddComponent<TargetBehaviour>();
        ms.registry.RegisterTarget(createdPrimitive, TargetType.Standard);

        var typeWithMethodInfo = Compilation.GenerateAllMethodsFromMonoType(dataModifierScript.GetType());
        ms.attacher.AttachCompiletimeMono(createdPrimitive, typeWithMethodInfo, dataModifierScript, "");
    }
}
