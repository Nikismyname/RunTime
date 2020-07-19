#region INIT

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionsTracker
{
    private MethodNode resultMethodCall;
    private List<ParameterDirectInput> paraDirectInputConnections = new List<ParameterDirectInput>();
    private List<ParameterMethod> paraMethConnections = new List<ParameterMethod>();

    private CubePersistance persistance;
    private List<ClassTracking> classTypeNamesForPersistance = new List<ClassTracking>();
    private List<DirectInput> directInputs = new List<DirectInput>(); 

    #endregion

    public ConnectionsTracker(WorldSpaceUI UI)
    {
        this.persistance = new CubePersistance(UI);
    }

    #region TRACKING

    public void TrackParameterAssignConstant(ParameterNode node, DirectInputNode constant)
    {
        var existing = paraDirectInputConnections.SingleOrDefault(x => x.Parameter == node);

        if (existing != null)
        {
            /// Assigning new constant to paramater, marking the previous constant as not used!
            existing.DirectInput.SetUsed(false, null);

            existing.DirectInput = constant;
            Debug.Log("replaced constatnt");
        }
        else
        {
            this.paraDirectInputConnections.Add(new ParameterDirectInput(node, constant));
        }
    }

    public void TrackParameterAssignMethod(ParameterNode node, MethodNode method)
    {
        var existing = paraMethConnections.SingleOrDefault(x => x.Parameter == node);

        if (existing != null)
        {
            existing.Method = method;
            Debug.Log("replaced method");
        }
        else
        {
            this.paraMethConnections.Add(new ParameterMethod(node, method));
        }
    }

    public void TrackResultAssignMethodCall(MethodNode node)
    {
        this.resultMethodCall = node;
    }

    #endregion

    #region PRINT_RESULTS

    public object PrintResult(Variable[] variables = null)
    {
        if (variables == null)
            variables = new Variable[0];

        if (this.resultMethodCall == null)
        {
            Debug.Log("Result Not Connected!");
            return null;
        }

        object result = this.PrintResultRec(this.resultMethodCall, variables);

        if (result != null)
        {
            Debug.Log("SUCCESS " + result.ToString());
        }

        return result;
    }

    private object PrintResultRec(MethodNode node, Variable[] variables)
    {
        List<object> values = new List<object>();

        foreach (var paramater in node.MyParamaters)
        {
            var constant = this.paraDirectInputConnections.Where(x => x.Parameter.ParameterInfo.ID == paramater.ID).ToArray();
            if (constant.Length > 1)
            {
                Debug.Log("MORE THANT ONE CONSTANT!!!!");
                Debug.Break();
            }

            if (constant.Length == 1)
            {
                if (constant[0].DirectInput.IsVariable())
                {
                    /// Finding the variable with the right name and getting it's value. The values are passed by the resultUI.
                    values.Add(variables.SingleOrDefault(x => x.Name == constant[0].DirectInput.VariableName)?.Value);
                }
                else
                {
                    values.Add(constant[0].DirectInput.GetVal());
                }

                ///TODO: FIX
                values[values.Count - 1] = Convert.ChangeType(values[values.Count - 1], paramater.Info.ParameterType);

                continue;
            }

            var method = this.paraMethConnections.Where(x => x.Parameter.ParameterInfo.ID == paramater.ID).ToArray();
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
                values.Add(PrintResultRec(method[0].Method, variables));
            }

            values[values.Count - 1] = Convert.ChangeType(values[values.Count - 1], paramater.Info.ParameterType);
        }

        object obj = node.Object;
        object[] par = values.ToArray();

        ///DEBUGGING
        Debug.Log("###########################################################################################");
        Debug.Log($"<<<<{node.Object.GetType().Name}>>>");
        Debug.Log($">>>{node.MyMethodInfo.Info.Name}<<<");
        for (int i = 0; i < node.MyParamaters.Length; i++)
        {
            var param = node.MyParamaters[i];

            Debug.Log($"{param.Info.ParameterType.Name} {par[i].ToString()}");
        }
        ///...

        object result = node.MyMethodInfo.Info.Invoke(obj, par);

        Debug.Log("PASSED");

        return result;
    }

    #endregion

    public void RegisterClassName(ClassTracking classInfo)
    {
        this.classTypeNamesForPersistance.Add(classInfo);
    }

    public void Persist()
    {
        MethodIDParamaterID[] methodParams = this.paraMethConnections
            .Select(x => new MethodIDParamaterID
            {
                MethodID = x.Method.ID,
                ParameterID = x.Parameter.ID,
            })
            .ToArray();

        DirectInputIDParamaterID[] directInputs = paraDirectInputConnections
            .Select(x => new DirectInputIDParamaterID
            {
                DirectInputID = x.DirectInput.ID,
                ParameterID = x.Parameter.ID,
            })
            .ToArray();

        ClassInfo[] infos = this.classTypeNamesForPersistance
            .Select(x => new ClassInfo
            {
                Name = x.Name,
                Position = x.node.transform.position,
            })
            .ToArray();

        this.persistance.Persist(infos, methodParams, this.directInputs.ToArray(), directInputs, resultMethodCall == null? (int?)null: resultMethodCall.ID);
    }

    public void LoadPersistedData()
    {
        this.persistance.LoadPersistedData();
    }

    public void RegisterDirectInput(DirectInput DI)
    {
        this.directInputs.Add(DI);
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