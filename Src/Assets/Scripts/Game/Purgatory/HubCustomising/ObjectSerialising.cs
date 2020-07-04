using System.IO;
using UnityEngine;

public static class ObjectSerialising
{
    public const string Path = "C:/Users/ASUS G751JY/Desktop";
    public const string Name = "SerialiseTest1";

    public static void SerialiseObject(PrimitiveObjectDataModifier[] ds,string path, string name)
    {
        path = Path;
        name = Name;
        ///Assuming that only dirty ones are sent//or overriding them all for now;

        var s = new PrimitiveObjectSerialiseData[ds.Length];

        for (int i = 0; i < ds.Length; i++)
        {
            s[i].color = ds[i].color;
            s[i].position = ds[i].position;
            s[i].scale = ds[i].scale;
            s[i].type = ds[i].type;
        }

        var serialisedTest = JsonUtility.ToJson(s); 

        File.WriteAllText(path + $"/{name}.txt", serialisedTest); 
    }

    public static PrimitiveObjectSerialiseData[] DeserialiseObjects(string path, string name)
    {
        path = Path; name = Name;

        var fullPath = path + $"/{name}.txt";

        if (!File.Exists(fullPath))
        {
            Debug.Log("File to deserialise not found!");
            return new PrimitiveObjectSerialiseData[0];
        }

        var text = File.ReadAllText(path+ $"/{name}.txt");

        var objs = JsonUtility.FromJson<PrimitiveObjectSerialiseData[]>(text);

        return objs;
    }
}
