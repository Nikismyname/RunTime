using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ActionKeyPersistance
{
    private const string fileName = "action_key_persistance.txt";
    private static JsonSerializerSettings settings;

    static ActionKeyPersistance()
    {
        settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    public static void Persist(ActionKeyPersistanceData data)
    {
        List<ActionKeyPersistanceData> datas = GetKeyCubeMapping().ToList();

        ActionKeyPersistanceData existingMapping = datas.SingleOrDefault(x => x.CubeName == data.CubeName);

        if (existingMapping != null)
        {
            existingMapping.CubeName = data.CubeName;
        }
        else
        {
            datas.Add(data);
        }


        File.WriteAllText(fileName, Serialize(datas));
    }

    public static void Delete(string name)
    {
        ActionKeyPersistanceData[] datas = GetKeyCubeMapping();

        ActionKeyPersistanceData existingMapping = datas.SingleOrDefault(x => x.CubeName == name);

        if (existingMapping == null)
        {
            Debug.LogError("The persisted action key mapping for deletion was not found!");
            return;
        }

        datas = datas.Where(x=> x.CubeName != name).ToArray();

        File.WriteAllText(fileName, Serialize(datas));
    }

    public static ActionKeyPersistanceData[] GetKeyCubeMapping()
    {
        if (File.Exists(fileName) == false)
        {
            var some = File.Create(fileName);
            some.Dispose();
        }

        string text = File.ReadAllText(fileName);

        ActionKeyPersistanceData[] infos = JsonConvert.DeserializeObject<ActionKeyPersistanceData[]>(text);

        infos = infos == null ? new ActionKeyPersistanceData[0] : infos;

        return infos;
    }

    public class ActionKeyPersistanceData
    {
        public int KeyId { get; set; }

        public string CubeName { get; set; }
    }

    private static string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None, settings);
    }
}