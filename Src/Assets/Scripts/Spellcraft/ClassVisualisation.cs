#region INIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ClassVisualisation
{
    private WorldSpaceUI UI;

    private int currParamId = 0;
    private int currMethodId = 0;

    public ClassVisualisation(WorldSpaceUI UI)
    {
        this.UI = UI;
    }

    public void Reset()
    {
        this.currMethodId = 0;
        this.currParamId = 0;
    }

    #endregion

    #region GENERATE_NODE_DATA

    public ClassNode GenerateNodeData<T>() where T : class, new()
    {
        Type type = typeof(T);
        PropertyInfo[] props = type.GetProperties();
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => !x.Name.StartsWith("set_") && !x.Name.StartsWith("get_")).ToArray();
        return new ClassNode(type, props, methods, new T());
    }
    
    public ClassNode GenerateNodeData(Type type) 
    {
        PropertyInfo[] props = type.GetProperties();
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => !x.Name.StartsWith("set_") && !x.Name.StartsWith("get_")).ToArray();
        return new ClassNode(type, props, methods, Activator.CreateInstance(type));
    }

    #endregion

    #region GENERATE_CLASS_VISUALISATION
    
    public MethodAndParameterNodes[] GenerateClassVisualisation(ClassNode node, Vector3 position, out Node nodeOut)
    {
        GameObject basy = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        basy.transform.SetParent(this.UI.spellcraftParent.transform);

        basy.SetColor(Color.red);
        basy.SetShader();

        Node nodeBeh = basy.AddComponent<Node>();
        nodeBeh.Setup(this.UI);
        nodeOut = nodeBeh;

        this.GeneratePropertyPips(node, basy);

        MethodAndParameterNodes[] nodes = this.GenerateMethodPips(node, basy);

        basy.transform.position = position;

        basy.transform.Rotate(new Vector3(1, 0, 0), 180f);

        return nodes; 
    }

    public void GeneratePropertyPips(ClassNode node, GameObject baseSphere)
    {
        for (int i = 0; i < node.Properties.Length; i++)
        {
            GameObject pip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pip.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            pip.transform.position = new Vector3(0, 0.5f, 0);
            pip.transform.parent = baseSphere.transform;
            pip.SetColor(Color.green);
            pip.SetShader();

            pip.RotateAroundUnitSphere(new Vector3(1, 0, 0), 30f);

            float oneDiv = 360f / node.Properties.Length;
            float wantedAngle = (float)i * oneDiv;

            pip.RotateAroundUnitSphere(new Vector3(0, 1, 0), wantedAngle);
        }
    }

    public MethodAndParameterNodes[] GenerateMethodPips(ClassNode node, GameObject baseSphere)
    {
        List<MethodAndParameterNodes> methodNodes = new List<MethodAndParameterNodes>();

        for (int j = 0; j < node.Methods.Length; j++)
        {
            MethodAndParameterNodes methodNode = new MethodAndParameterNodes();

            float initialOffset = 50f;
            MethodInfo method = node.Methods[j];
            GameObject methodPip = CreatePip(baseSphere, Color.white);
            ParameterInfo[] rawParamInfos = method.GetParameters().ToArray();

            methodPip.RotateAroundUnitSphere(new Vector3(1, 0, 0), initialOffset);
            float oneDivAround = 360f / node.Methods.Length;
            float wantedAngleAround = (float)j * oneDivAround;
            methodPip.RotateAroundUnitSphere(new Vector3(0, 1, 0), wantedAngleAround);

            List<MyParameterInfo> myParamaterInfos = new List<MyParameterInfo>();
            for (int i = 0; i < rawParamInfos.Length; i++)
            {
                MyParameterInfo param = new MyParameterInfo(this.currParamId++, rawParamInfos[i]);
                myParamaterInfos.Add(param);
                GameObject paramaterPip = CreatePip(baseSphere, Color.black);
                ParameterNode paramScript = paramaterPip.AddComponent<ParameterNode>();
                paramScript.Setup(param, method, node.Object, this.UI);
                methodNode.Parameters.Add(paramScript);
                float wantedAngleDown = initialOffset + (i + 1) * 20f;
                paramaterPip.RotateAroundUnitSphere(new Vector3(1, 0, 0), wantedAngleDown);
                paramaterPip.RotateAroundUnitSphere(new Vector3(0, 1, 0), wantedAngleAround);
            }

            var methodScript = methodPip.AddComponent<MethodNode>();
            methodScript.Setup(new MyMethodInfo(this.currMethodId++, method), myParamaterInfos.ToArray(), node.Object, this.UI);
            methodNode.Method = methodScript;

            methodNodes.Add(methodNode);
        }

        return methodNodes.ToArray();
    }

    private GameObject CreatePip(GameObject parent, Color color, float scale = 0.1f)
    {
        GameObject pip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pip.transform.localScale = new Vector3(scale, scale, scale);
        pip.transform.position = new Vector3(0, 0.5f, 0);
        pip.transform.parent = parent.transform;
        pip.SetColor(color);
        pip.SetShader();
        return pip;
    }

    #endregion

    #region DATA_CLASSES

    public class MethodAndParameterNodes
    {
        public MethodNode Method { get; set; }
        public List<ParameterNode> Parameters { get; set; } = new List<ParameterNode>();
    }
}

public class ClassNode
{
    public ClassNode(Type type, PropertyInfo[] properties, MethodInfo[] methods, object @object)
    {
        Type = type;
        Properties = properties;
        Methods = methods;
        Object = @object;
    }

    public Type Type { get; set; }
    public PropertyInfo[] Properties { get; set; }
    public MethodInfo[] Methods { get; set; }
    public object Object { get; set; }
}

#endregion