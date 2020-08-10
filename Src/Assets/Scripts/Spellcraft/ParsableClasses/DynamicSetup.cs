using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSetup
{
    private List<Type> nodeTypes = new List<Type>();
    private ClassVisualisation classVisualisation;
    private WorldSpaceUI UI;
    //private ResultCanvas resultCanvas;
    private InputCanvas inputCanvas;

    public static Type[] Types = new Type[] {typeof(LineDestroyer), typeof(Projectile), typeof(Teleporter), typeof(Vector3Classes.Vector3Util)};

    public DynamicSetup(ClassVisualisation classVisualisation, WorldSpaceUI UI, ResultCanvas resultCanvas, InputCanvas inputCanvas)
    {
        this.classVisualisation = classVisualisation;
        this.UI = UI;
        //this.resultCanvas = resultCanvas;
        this.inputCanvas = inputCanvas;
    }

    public void RegisterNode<T>(Type type = null, Vector3? position = null)
    {
        this.nodeTypes.Add(type);

        position = position ?? new Vector3(0, 0, 0);

        // creating the nodes
        ClassVisualisation.MethodAndParameterNodes[] spellClass = this.classVisualisation.GenerateClassVisualisation(
            this.classVisualisation.GenerateNodeData<Teleporter>(), position.Value, out Node node);

        // registering them for persistence 
        this.UI.connTracker.RegisterClassNameForPersistance(
            new ClassTracking {Name = typeof(Teleporter).FullName, node = node}, null);
    }

    private int id = 0; 
    
    public void AddVariable(string varName)
    {
        var variable = this.inputCanvas.CreateInputCanvas(default, ++id, this.UI, true, varName);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, varName, null), null);
    }

    public void AddConstant(object value)
    {
        var contant = this.inputCanvas.CreateInputCanvas(value, ++id, this.UI, false);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, null, value), null);
    }
}