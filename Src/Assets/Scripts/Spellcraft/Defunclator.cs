using Boo.Lang;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Defunclator
{
    private float marginY = 0.1f;
    private float marginX = 0.1f;
    private float methodCube = 0.3f;
    private float methodSphere = 0.2f;
    private float propertySphere = 0.3f;
    private WorldSpaceUI UI;

    private int currParamId = 0;

    public Defunclator(WorldSpaceUI ui)
    {
        this.UI = ui;
    }

    public ClassNode GenerateNodeData<T>() where T : class, new()
    {
        Type type = typeof(T);
        ///TODO: Maybe mechanic where the private ones do different thing!!!
        PropertyInfo[] props = type.GetProperties();
        //MethodInfo[] methods = type.GetMethods();
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => !x.Name.StartsWith("set_") && !x.Name.StartsWith("get_")).ToArray();
        //Debug.Log(string.Join(", ", methods.Select(x=>x.Name).ToArray()));
        return new ClassNode(type, props, methods, new T());
    }

    public GameObject BuildUI(ClassNode node)
    {
        GameObject parent = new GameObject(node.Type.Name);

        int propCount = node.Properties.Length;
        int methodCount = node.Methods.Length;

        for (int i = 0; i < node.Properties.Length; i++)
        {
            var prop = node.Properties[i];
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            PropertyUINode beh = p.AddComponent<PropertyUINode>();
            p.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            p.transform.position = new Vector3(this.GetPropX(i, propCount), 1, 0);
            p.transform.parent = parent.transform;
        }

        for (int i = 0; i < node.Methods.Length; i++)
        {
            var method = node.Methods[i];
        }

        return null;
    }

    public void GenerateClassVisualisation(ClassNode node, Vector3 position)
    {
        GameObject basy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        basy.SetColor(Color.red);
        basy.SetShader();

        basy.AddComponent<Node>().Setup(this.UI);

        this.GeneratePropertyPips(node, basy);

        this.GenerateMethodPips(node, basy);

        basy.transform.position = position;

        basy.transform.Rotate(new Vector3(1,0,0), 180f); 
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

    public void GenerateMethodPips(ClassNode node, GameObject baseSphere)
    {
        for (int j = 0; j < node.Methods.Length; j++)
        {
            float initialOffset = 50f; 
            var method = node.Methods[j];
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
                paramaterPip.AddComponent<ParameterNode>().Setup(param, node.Object, this.UI);
                float wantedAngleDown = initialOffset + (i + 1) * 20f;
                paramaterPip.RotateAroundUnitSphere(new Vector3(1, 0, 0), wantedAngleDown);
                paramaterPip.RotateAroundUnitSphere(new Vector3(0, 1, 0), wantedAngleAround);
            }

            methodPip.AddComponent<MethodNode>().Setup(method, myParamaterInfos.ToArray(), node.Object, this.UI);
        }
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

    private float GetPropX(int index, int count)
    {
        float propLenght = count * propertySphere + (count - 1) * marginX;

        float result = index * (this.propertySphere + this.marginX) + this.propertySphere / 2 - propLenght / 2;

        return result;
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

