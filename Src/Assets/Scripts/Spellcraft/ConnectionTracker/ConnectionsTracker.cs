#region INIT

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionsTracker
{
    private const string workBundleName = "WorkingOn";

    private List<CubeBundle> bundles = new List<CubeBundle>();
    private CubePersistance persistance;

    public ConnectionsTracker(WorldSpaceUI UI)
    {
        this.persistance = new CubePersistance(UI);
    }

    public void RegisterBundle(string name)
    {
        name = name == null ? workBundleName : name;

        this.bundles.Add(new CubeBundle(name));
    }

    public void Reset()
    {
        //this.resultMethodCall = null;
        //this.paraDirectInputConnections = new List<ParameterDirectInput>();
        //this.paraMethConnections = new List<ParameterMethod>();
        //this.classTypeNamesForPersistance = new List<ClassTracking>();
        //this.directInputs = new List<DirectInput>();
    }

    #endregion

    #region TRACKING

    public void TrackParameterAssignConstant(ParameterNode node, DirectInputNode constant, string name = null)
    {
        name = name == null ? workBundleName : name;


        var existing = this.bundles.Single(x => x.Name == name)?.ParaDirectInputConnections
            .SingleOrDefault(x => x.Parameter == node);

        if (existing != null)
        {
            /// Assigning new constant to paramater, marking the previous constant as not used!
            existing.DirectInput.SetUsed(false, null);

            existing.DirectInput = constant;
            Debug.Log("replaced constatnt");
        }
        else
        {
            this.bundles.Single(x => x.Name == name)?.ParaDirectInputConnections
                .Add(new ParameterDirectInput(node, constant));
        }
    }

    public void TrackParameterAssignMethod(ParameterNode node, MethodNode method, string name = null)
    {
        name = name == null ? workBundleName : name;

        var existing = this.bundles.Single(x => x.Name == name)?.ParaMethConnections
            .SingleOrDefault(x => x.Parameter == node);

        if (existing != null)
        {
            existing.Method = method;
            Debug.Log("replaced method");
        }
        else
        {
            this.bundles.Single(x => x.Name == name)?.ParaMethConnections.Add(new ParameterMethod(node, method));
        }
    }

    public void TrackResultAssignMethodCall(MethodNode node, string name = null)
    {
        name = name == null ? workBundleName : name;

        this.bundles.Single(x => x.Name == name).ResultMethodCall = node;
    }

    #endregion

    #region PRINT_RESULTS

    public object PrintResult(string bundleName, Variable[] variables = null)
    {
        bundleName = bundleName == null ? workBundleName : bundleName;

        CubeBundle bundle = this.bundles.SingleOrDefault(x => x.Name == bundleName);

        if (bundle == null)
        {
            this.bundles.Add(new CubeBundle(bundleName));
            this.LoadPersistedData(bundleName, false);
            Debug.Log($"Bundle not Found, Loading Bundle: {bundleName}");
        }

        bundle = this.bundles.SingleOrDefault(x => x.Name == bundleName);

        if (bundle == null)
        {
            this.LoadPersistedData(bundleName, false);
            Debug.LogError($"Bundle not Found After Loading it, ABORT");
            return null;
        }

        if (variables == null)
            variables = new Variable[0];

        if (bundle.ResultMethodCall == null)
        {
            Debug.Log("Result Not Connected!");
            return null;
        }

        object result = this.PrintResultRec(bundle.ResultMethodCall, bundle, variables);

        if (result != null)
        {
            Debug.Log("SUCCESS " + result.ToString());
        }

        return result;
    }

    private object PrintResultRec(MethodNode node, CubeBundle bundle, Variable[] variables)
    {
        List<object> values = new List<object>();

        foreach (var paramater in node.MyParamaters)
        {
            var constant = bundle.ParaDirectInputConnections.Where(x => x.Parameter.ParameterInfo.ID == paramater.ID)
                .ToArray();
            if (constant.Length > 1)
            {
                Debug.Log("MORE THANT ONE CONSTANT!!!!");
                Debug.Break();
            }

            if (constant.Length == 1)
            {
                if (constant[0].DirectInput.IsVariable())
                {
                    // Finding the variable with the right name and getting it's value. The values are passed by the resultUI.
                    values.Add(variables.SingleOrDefault(x => x.Name == constant[0].DirectInput.VariableName)?.Value);
                }
                else
                {
                    values.Add(constant[0].DirectInput.GetVal());
                }

                try
                {
                    values[values.Count - 1] =
                        Convert.ChangeType(values[values.Count - 1], paramater.Info.ParameterType);
                }
                catch (Exception e)
                {
                    // Debug.Log("Error converting");
                }

                continue;
            }

            var method = bundle.ParaMethConnections.Where(x => x.Parameter.ParameterInfo.ID == paramater.ID).ToArray();
            if (method.Length > 1)
            {
                Debug.Log("MORE THANT ONE CONSTANT!!!!");
                Debug.Break();
            }

            if (method.Length == 0)
            {
                /// Hack to let me only pass one float of many and the rest to be 0 
                values.Add(paramater.Info.DefaultValue);
                Debug.Log("Property not assigned to - Giving it the default!");
            }
            else
            {
                values.Add(PrintResultRec(method[0].Method, bundle, variables));
            }

            values[values.Count - 1] = Convert.ChangeType(values[values.Count - 1], paramater.Info.ParameterType);
        }

        object obj = node.Object;
        object[] par = values.ToArray();

        //DEBUGGING
        // Debug.Log("###########################################################################################");
        // Debug.Log($"<<<<{node.Object.GetType().Name}>>>");
        // Debug.Log($">>>{node.MyMethodInfo.Info.Name}<<<");
        for (int i = 0; i < node.MyParamaters.Length; i++)
        {
            var param = node.MyParamaters[i];

            Debug.Log($"{param.Info.ParameterType.Name} {par[i]?.ToString()}");
        }
        //...

        object result = node.MyMethodInfo.Info.Invoke(obj, par);

        //Debug.Log("PASSED");

        return result;
    }

    #endregion

    public void RegisterClassNameForPersistance(ClassTracking classInfo, string name)
    {
        name = name == null ? workBundleName : name;

        this.bundles.Single(x => x.Name == name).ClassTypeNamesForPersistance.Add(classInfo);
    }

    public void Persist(string name = "some_name")
    {
        MethodIDParamaterID[] methodParams = this.bundles.Single(x => x.Name == workBundleName).ParaMethConnections
            .Select(x => new MethodIDParamaterID
            {
                MethodID = x.Method.ID,
                ParameterID = x.Parameter.ID,
            })
            .ToArray();

        DirectInputIDParamaterID[] directInputs = this.bundles.Single(x => x.Name == workBundleName)
            .ParaDirectInputConnections
            .Select(x => new DirectInputIDParamaterID
            {
                DirectInputID = x.DirectInput.ID,
                ParameterID = x.Parameter.ID,
            })
            .ToArray();

        ClassInfo[] infos = this.bundles.Single(x => x.Name == workBundleName).ClassTypeNamesForPersistance
            .Select(x => new ClassInfo
            {
                Name = x.Name,
                Position = x.node.transform.position,
            })
            .ToArray();

        var resultM = this.bundles.Single(x => x.Name == workBundleName).ResultMethodCall;

        this.persistance.Persist(
            infos,
            methodParams,
            this.bundles.Single(x => x.Name == workBundleName).DirectInputs.ToArray(),
            directInputs,
            name,
            resultM?.ID);
    }

    public void LoadPersistedData(string name, bool visualise = false)
    {
        this.persistance.LoadPersistedData(name, visualise);
    }

    public void RegisterDirectInput(DirectInput DI, string name)
    {
        name = name == null ? workBundleName : name;

        this.bundles.Single(x => x.Name == name).DirectInputs.Add(DI);
    }

    public class CubeBundle
    {
        public CubeBundle(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; set; }
        public MethodNode ResultMethodCall { get; set; }
        public List<ParameterDirectInput> ParaDirectInputConnections { get; set; } = new List<ParameterDirectInput>();
        public List<ParameterMethod> ParaMethConnections { get; set; } = new List<ParameterMethod>();
        public List<ClassTracking> ClassTypeNamesForPersistance { get; set; } = new List<ClassTracking>();
        public List<DirectInput> DirectInputs { get; set; } = new List<DirectInput>();
    }
}

#region DATA_CLASSES

public class ParameterDirectInput
{
    public ParameterNode Parameter { get; set; }

    public DirectInputNode DirectInput { get; set; }

    public ParameterDirectInput(ParameterNode parameter, DirectInputNode constant)
    {
        Parameter = parameter;
        DirectInput = constant;
    }
}

public class ParameterMethod
{
    public ParameterNode Parameter { get; set; }

    public MethodNode Method { get; set; }

    public ParameterMethod(ParameterNode parameter, MethodNode method)
    {
        Parameter = parameter;
        Method = method;
    }
}

public class Variable
{
    public string Name { get; set; }

    public object Value { get; set; }
}

public class ClassTracking
{
    public string Name { get; set; }

    public Node node { get; set; }
}

#endregion