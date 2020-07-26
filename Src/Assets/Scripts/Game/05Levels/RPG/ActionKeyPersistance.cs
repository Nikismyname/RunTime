using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ActionKeyPersistance
{
    private const string fileName = "action_key_persistance.txt";
    private JsonSerializerSettings settings;

    public ActionKeyPersistance()
    {
        this.settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    public void Persist(ActionKeyPersistanceData data)
    {
        List<ActionKeyPersistanceData> datas = GetKeyCubeMapping().ToList();

        ActionKeyPersistanceData existingMapping = datas.SingleOrDefault(x => x.CubeName == data.CubeName); 

        if(existingMapping != null)
        {
            existingMapping.CubeName = data.CubeName;
        }
        else
        {
            datas.Add(data);
        }


        File.WriteAllText(fileName, this.Serialize(datas));
    }

    public ActionKeyPersistanceData[] GetKeyCubeMapping()
    {
        string text = File.ReadAllText(fileName);

        ActionKeyPersistanceData[] infos = JsonConvert.DeserializeObject<ActionKeyPersistanceData[]>(text);

        return infos;
    }

    public class ActionKeyPersistanceData
    {
        public int KeyId { get; set; }

        public string  CubeName { get; set; }
    }

    private string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None, this.settings);
    }
}

