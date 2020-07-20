#region INIT

using Boo.Lang;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.XR;

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

    public void Persist(ClassInfo[] classTypesAndPosInOrder, MethodIDParamaterID[] methodParams, DirectInput[] directInputs, DirectInputIDParamaterID[] directInputIDsParamID, int? resultMethodId)
    {
        CubeInfo info = new CubeInfo
        {
            ID = 1,
            Name = "2",
            ClassInfos = classTypesAndPosInOrder,
            MethsParams = methodParams,
            DirectInputParamater = directInputIDsParamID,
            directInputs = directInputs,
            ResultMethodID = resultMethodId,
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

        List<ClassVisualisation.MethodAndParameterNodes[]> methodInfoWithParamInfo = new List<ClassVisualisation.MethodAndParameterNodes[]>(); 

        /// Creating and regestering the class nodes
        for (int i = 0; i < info.ClassInfos.Length; i++)
        {
            ClassInfo cin = info.ClassInfos[i];
            Type type = Assembly.GetExecutingAssembly().GetType(cin.Name);
            Vector3 position = cin.Position;

            var spellClass = this.UI.classVisualisation.GenerateClassVisualisation(this.UI.classVisualisation.GenerateNodeData(type), position, out Node one);
            this.UI.connTracker.RegisterClassName(new ClassTracking { Name = type.FullName, node = one });
            methodInfoWithParamInfo.Add(spellClass);
        }
        ///... 

        List<InputCanvas.InputElements> inputs = new List<InputCanvas.InputElements>();

        /// Establish the DirectInputs
        List<ResultCanvas.VariableInput> variablesForDisplay = new List<ResultCanvas.VariableInput>();
        for (int i = 0; i < info.directInputs.Length; i++)
        {
            DirectInput DI = info.directInputs[i];

            /// CONSTANT
            if (DI.Name == null)
            {
                var cnst = this.UI.inputCanvas.CreateInputCanvas(DI.Value, DI.ID, this.UI, false);
                this.UI.connTracker.RegisterDirectInput(new DirectInput(DI.ID, null, DI.Value));
                inputs.Add(cnst);
            }
            else /// VARIABLE
            {
                var var = this.UI.inputCanvas.CreateInputCanvas(default, DI.ID, this.UI, true, DI.Name);
                this.UI.connTracker.RegisterDirectInput(new DirectInput(DI.ID, DI.Name, null));
                inputs.Add(var);
                variablesForDisplay.Add(new ResultCanvas.VariableInput(typeof(float), DI.Name));
            }
        }

        this.UI.resultCanvas.SetVariables(variablesForDisplay.ToArray());
        ///...

        /// Parameter Method Connections!
        for (int i = 0; i < info.MethsParams.Length; i++)
        {
            MethodIDParamaterID MP = info.MethsParams[i];

            MethodNode mNode = methodInfoWithParamInfo.SelectMany(x => x.Select(y => y.Method)).Single(x=> x.ID == MP.MethodID); 
            ParameterNode pNode = methodInfoWithParamInfo.SelectMany(x => x.SelectMany(y=>y.Parameters)).Single(x => x.ID == MP.MethodID);

            _ = this.UI.connRegisterer.RegisterParameterClick(pNode,mNode);
        }
        ///... 

        for (int i = 0; i < info.DirectInputParamater.Length; i++)
        {
            DirectInputIDParamaterID DP = info.DirectInputParamater[i];

            ParameterNode pNode = methodInfoWithParamInfo.SelectMany(x => x.SelectMany(y => y.Parameters)).Single(x => x.ID == DP.ParameterID);
            DirectInputNode DINode = inputs.Select(x=> x.Node).Single(x => x.ID == DP.DirectInputID);

            this.UI.connRegisterer.RegisterConstantClick(DINode, pNode);
        }

        MethodNode resultMethod = methodInfoWithParamInfo.SelectMany(x => x.Select(y => y.Method)).Single(x => x.ID == info.ResultMethodID);
        this.UI.connRegisterer.RegisterResultClick(this.UI.resultNode, resultMethod);
        this.UI.inputCanvas.InputsHide();
    }

    public static CubeInfo[] GetAllSavedCubes()
    {
        string[] texts = File.ReadAllLines(fileName).Where(x=> string.IsNullOrWhiteSpace(x) == false).ToArray();

        CubeInfo[] infos = texts.Select(x => JsonConvert.DeserializeObject<CubeInfo>(x)).ToArray();

        return infos;
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
    public DirectInput[] directInputs;
    public int? ResultMethodID; 

    public MethodIDParamaterID[] MethsParams;
    public DirectInputIDParamaterID[] DirectInputParamater;
}

public class DirectInputIDParamaterID
{
    public int DirectInputID;
    public int ParameterID;
}

public class DirectInput
{
    public int ID;
    public string Name;
    public object Value;

    public DirectInput(int directInputID, string name, object value)
    {
        ID = directInputID;
        Name = name;
        Value = value;
    }
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
