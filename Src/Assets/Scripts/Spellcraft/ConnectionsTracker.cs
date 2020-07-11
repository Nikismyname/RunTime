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

    public void TrackParameterAssignConstant(ParameterNode node, ConstantNode constant)
    {
        this.paraConstConnections.Add(new ParameterConstant(node, constant));
    }

    public void TrackResultAssignMethodCall(MethodNode node)
    {
        this.resultMethodCall = node;
    }

    //public void TrackParameterAssignedVariable()
    //{

    //}

    //public void TrackParamaterAssignedMethodReturn()
    //{

    //}

    public void PrintResult()
    {
        //foreach (var conn in this.paraConstConnections)
        //{
        //var par = conn.Parameter;
        //object val = conn.Constant.GetVal();

        //par.ParameterInfo.  par.Object;
        //}

        if(resultMethodCall == null)
        {
            Debug.Log("Result Not Connected!");
            return;
        }

        List<object> values = new List<object>(); 

        foreach (var item in this.resultMethodCall.MyParamaters)
        {
            var some = this.paraConstConnections.Where(x=> x.Parameter.ParameterInfo == item).ToArray();
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
}

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
