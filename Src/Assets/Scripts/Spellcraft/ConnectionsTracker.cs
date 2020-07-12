#region INIT

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionsTracker
{
    InputVariable[] inputVariables;
    ConstantNode[] inputConstants;
    ResultConstant result;
    MethodNode resultMethodCall;

    PropertyNode[] propertyNodes;
    MethodNode[] methodNodes;
    ParameterNode[] parameterNodes;

    List<ParameterConstant> paraConstConnections = new List<ParameterConstant>();
    List<ParameterMethod> paraMethConnections = new List<ParameterMethod>();
    #endregion

    #region TRACKING

    public void TrackParameterAssignConstant(ParameterNode node, ConstantNode constant)
    {
        var existing = paraConstConnections.SingleOrDefault(x => x.Parameter == node);

        if (existing != null)
        {
            /// Assigning new constant to paramater, marking the previous constant as not used!
            existing.Constant.SetUsed(false, null);

            existing.Constant = constant;
            Debug.Log("replaced constatnt");
        }
        else
        {
            this.paraConstConnections.Add(new ParameterConstant(node, constant));
        }
    }

    public void TrackParameterAssignMethod(ParameterNode node, MethodNode method)
    {
        var existing = paraMethConnections.SingleOrDefault(x => x.Parameter == node);

        if (existing != null)
        {
            /// Assigning new constant to paramater, marking the previous constant as not used!
            //existing.Constant.SetUsed(false, null);

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

    public void PrintResult()
    {
        if (this.resultMethodCall == null)
        {
            Debug.Log("Result Not Connected!");
            return;
        }

        object result = this.PrintResultRec(this.resultMethodCall);

        Debug.Log("SUCCESS " + result.ToString());
    }

    private object PrintResultRec(MethodNode node)
    {
        List<object> values = new List<object>();

        foreach (var paramater in node.MyParamaters)
        {
            var constant = this.paraConstConnections.Where(x => x.Parameter.ParameterInfo.ID == paramater.ID).ToArray();
            if (constant.Length > 1)
            {
                Debug.Log("MORE THANT ONE CONSTANT!!!!");
                Debug.Break();
            }

            if (constant.Length == 1)
            {
                values.Add(constant[0].Constant.GetVal());
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
                Debug.Log("Property not assigned to!!!");
                Debug.Break();
            }

            values.Add(PrintResultRec(method[0].Method));
        }

        object obj = node.Object;
        object[] par = values.ToArray();
        object result = node.MethodInfo.Invoke(obj, par);

        return result;
    }

    #endregion
}

#region DATA_CLASSES

public class InputVariable
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public object Value { get; set; }
}

public class InputConstant : InputVariable { }

public class ResultConstant : InputVariable { }

public class ResultVariable : InputVariable { }

public class ParameterConstant
{
    public ParameterNode Parameter { get; set; }

    public ConstantNode Constant { get; set; }

    public ParameterConstant(ParameterNode parameter, ConstantNode constant)
    {
        Parameter = parameter;
        Constant = constant;
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

#endregion