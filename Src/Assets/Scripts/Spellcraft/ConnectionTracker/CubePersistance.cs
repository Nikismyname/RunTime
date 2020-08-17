#region INIT

using Boo.Lang;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CubePersistance
{
    private static JsonSerializerSettings settings;
    private const string fileName = "persistance.txt";
    private WorldSpaceUI UI;

    public CubePersistance(WorldSpaceUI UI)
    {
        this.UI = UI;
    }

    static CubePersistance()
    {
        settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    #endregion

    public void Persist(ClassInfo[] classTypesAndPosInOrder, MethodIDParamaterID[] methodParams, DirectInput[] directInputs, DirectInputIDParamaterID[] directInputIDsParamID, string name, int? resultMethodId)
    {
        CubeInfo info = new CubeInfo
        {
            ID = 1,
            Name = name,
            ClassInfos = classTypesAndPosInOrder,
            MethsParams = methodParams,
            DirectInputParamater = directInputIDsParamID,
            directInputs = directInputs,
            ResultMethodID = resultMethodId,
        };

        string text = Serialize(info);

        File.AppendAllLines(fileName, new string[]
        {
            text,
        });
    }

    public void LoadPersistedData(string name, bool visualise = false)
    {
        /// Reseting the ID for every new deserialized item so the ids match the original ids.
        this.UI.classVisualisation.Reset();

        CubeInfo[] infos = GetAllSavedCubes();

        CubeInfo info = infos.FirstOrDefault(x => x.Name == name);
        if (info == null)
        {
            Debug.Log("No info with the given name!!!");
            return;
        }

        List<ClassVisualisation.MethodAndParameterNodes[]> methodInfoWithParamInfo = new List<ClassVisualisation.MethodAndParameterNodes[]>();

        /// Creating and regestering the class nodes
        for (int i = 0; i < info.ClassInfos.Length; i++)
        {
            ClassInfo cin = info.ClassInfos[i];
            Type type = Assembly.GetExecutingAssembly().GetType(cin.Name);
            Vector3 position = cin.Position;

            var spellClass = this.UI.classVisualisation.GenerateClassVisualisation(this.UI.classVisualisation.GenerateNodeData(type), position, out Node one);

            this.UI.connTracker.RegisterClassNameForPersistence(new ClassTracking { Name = type.FullName, node = one }, info.Name);

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
                this.UI.connTracker.RegisterDirectInput(new DirectInput(DI.ID, null, DI.Value), info.Name);
                inputs.Add(cnst);
            }
            else /// VARIABLE
            {
                var var = this.UI.inputCanvas.CreateInputCanvas(default, DI.ID, this.UI, true, DI.Name);
                this.UI.connTracker.RegisterDirectInput(new DirectInput(DI.ID, DI.Name, null), info.Name);
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

            MethodNode mNode = methodInfoWithParamInfo.SelectMany(x => x.Select(y => y.Method)).Single(x => x.ID == MP.MethodID);
            ParameterNode pNode = methodInfoWithParamInfo.SelectMany(x => x.SelectMany(y => y.Parameters)).Single(x => x.ID == MP.MethodID);

            if (visualise)
            {
                _ = this.UI.connRegisterer.RegisterParameterClick(pNode, mNode);
            }
            else
            {
                this.UI.connTracker.TrackParameterAssignMethod(pNode, mNode, info.Name);
            }
        }
        ///... 

        for (int i = 0; i < info.DirectInputParamater.Length; i++)
        {
            DirectInputIDParamaterID DP = info.DirectInputParamater[i];

            ParameterNode[] parameters = methodInfoWithParamInfo.SelectMany(x => x.SelectMany(y => y.Parameters)).ToArray();
            ParameterNode pNode = parameters.Single(x => x.ID == DP.ParameterID);
            
            DirectInputNode DINode = inputs.Select(x => x.Node).Single(x => x.ID == DP.DirectInputID);

            if (visualise)
            {
                this.UI.connRegisterer.RegisterConstantClick(DINode, pNode);
            }
            else
            {
                this.UI.connTracker.TrackParameterAssignConstant(pNode, DINode, info.Name);
            }
        }

        MethodNode[] methodNodes = methodInfoWithParamInfo.SelectMany(x => x.Select(y => y.Method)).ToArray();

        MethodNode[] resultMethods = methodNodes.Where(x => x.ID == info.ResultMethodID.Value).ToArray();

        if(resultMethods.Length == 0)
        {
            Debug.Log("Result Node not found!");
            return; 
        }

        if(resultMethods.Length > 1)
        {
            Debug.Log("More than one Result Node!!");
            return;
        }

        MethodNode resultMethod = resultMethods[0];

        if (visualise)
        {
            this.UI.connRegisterer.RegisterResultClick(this.UI.resultNode, resultMethod);
        }
        else
        {
            this.UI.connTracker.TrackResultAssignMethodCall(resultMethod, info.Name);
        }

        this.UI.inputCanvas.InputsHide();
    }

    public static void DeleteCube(string name)
    {
        CubeInfo[] cubes = GetAllSavedCubes();

        CubeInfo existing = cubes.SingleOrDefault(x => x.Name == name);

        if (existing == null)
        {
            Debug.Log("Cube to delete not found!");
            return;
        }

        cubes = cubes.Where(x => x.Name != name).ToArray();

        string[] lines = cubes.Select(x => Serialize(x)).ToArray();

        File.WriteAllLines(fileName, lines);
    }

    public static CubeInfo[] GetAllSavedCubes()
    {
        string[] texts = File.ReadAllLines(fileName).Where(x => string.IsNullOrWhiteSpace(x) == false).ToArray();

        CubeInfo[] infos = texts.Select(x => JsonConvert.DeserializeObject<CubeInfo>(x)).ToArray();

        return infos;
    }

    #region HELPERS

    private static string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None, settings);
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
    public string Name { get; set; }
    public Vector3 Position { get; set; }
}

#endregion 
