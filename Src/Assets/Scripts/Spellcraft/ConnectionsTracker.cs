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

    public void TrackResultAssignMethodCall(MethodNode node)
    {
        this.resultMethodCall = node;
    }

    #endregion

    #region PRINT_RESULTS

    public void PrintResult()
    {
        if (resultMethodCall == null)
        {
            Debug.Log("Result Not Connected!");
            return;
        }

        List<object> values = new List<object>();

        foreach (var item in this.resultMethodCall.MyParamaters)
        {
            var some = this.paraConstConnections.Where(x => x.Parameter.ParameterInfo == item).ToArray();
            if (some.Length == 0)
            {
                Debug.Log("Param matching wrong!! no const!!");
            }
            if (some.Length > 1)
            {
                Debug.Log("Param matching wrong!! many const!!");
            }

            values.Add(some[0].Constant.GetVal());
        }

        object obj = this.resultMethodCall.Object;
        object[] par = values.ToArray();

        object result = this.resultMethodCall.MethodInfo.Invoke(obj, par);

        Debug.Log("SUCCESS " + result.ToString());
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

#endregion