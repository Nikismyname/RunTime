using System;
using System.Collections.Generic;

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
