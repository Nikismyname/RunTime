#region INIT

using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CubePersistance
{
    private const string fileName = "persistance.txt";
    private JsonSerializerSettings settings;
    private WorldSpaceUI UI; 

    public CubePersistance(WorldSpaceUI UI)
    {
        this.UI = UI;

        this.settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    public void Persist(ClassInfo[] classTypesAndPosInOrder, MethodIDParamaterID[] methodParams, DirectInputIDParamaterID[] directInputs)
    {

        CubeInfo info = new CubeInfo
        {
            ID = 1,
            Name = "2",
            ClassInfos = classTypesAndPosInOrder,
            MethsParams = methodParams,
            DirectInputs = directInputs,
        };

        string text = this.Serialize(info);

        File.AppendAllLines(fileName, new string[]
        {
            text,
        });
    }

    public void LoadPersistedData()
    {
        string text = File.ReadAllLines(fileName).Last();
        CubeInfo info = JsonConvert.DeserializeObject<CubeInfo>(text);

        for (int i = 0; i < info.ClassInfos.Length; i++)
        {
            ClassInfo cin = info.ClassInfos[i];
            Type type = Assembly.GetExecutingAssembly().GetType(cin.Name);
            Vector3 position = cin.Position;

            var spellClass = this.UI.classVisualisation.GenerateClassVisualisation(this.UI.classVisualisation.GenerateNodeData(type), position, out Node one);
            this.UI.connTracker.RegisterClassName(new ClassTracking { Name = typeof(SpellcraftClasses.Projectile).FullName, node = one });
        }
    }

    private string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None, this.settings);
    }

    #endregion

    #region }
}

#endregion

#region DATA_CLASSES

public class CubeInfo
{
    public int ID;
    public string Name;
    public ClassInfo[] ClassInfos;

    public MethodIDParamaterID[] MethsParams;
    public DirectInputIDParamaterID[] DirectInputs;
}

public class DirectInputIDParamaterID
{
    public int DirectInputID;
    public int ParameterID;
    public object Value;
}

public class MethodIDParamaterID
{
    public int MethodID;
    public int ParameterID;
}

public class ClassInfo
{
    public string  Name { get; set; }
    public Vector3 Position { get; set; }
}

#endregion 
