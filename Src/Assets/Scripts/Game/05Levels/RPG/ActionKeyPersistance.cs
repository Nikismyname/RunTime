using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public static ActionKeyPersistanceData[] GetKeyCubeMapping()
    {
        if(File.Exists(fileName) == false)
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

