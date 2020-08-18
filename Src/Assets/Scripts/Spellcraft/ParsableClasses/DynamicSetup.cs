using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSetup
{
    private List<Type> nodeTypes = new List<Type>();
    private ClassVisualisation classVisualisation;
    private WorldSpaceUI UI;
    private InputCanvas inputCanvas;
    private Vector3 posOffset;

    public static Type[] Types = new Type[] {typeof(LineDestroyer), typeof(SpellcraftClasses.Projectile), typeof(Teleporter), typeof(Vector3Classes.Vector3Util)};

    public DynamicSetup(ClassVisualisation classVisualisation, WorldSpaceUI UI, ResultCanvas resultCanvas, InputCanvas inputCanvas)
    {
        this.classVisualisation = classVisualisation;
        this.UI = UI;
        //this.resultCanvas = resultCanvas;
        this.inputCanvas = inputCanvas;
        this.posOffset = new Vector3(5,5, 5);
    }

    public void RegisterNode(Type type)
    {
        this.nodeTypes.Add(type);

        Vector3 position = this.UI.spellcraftParent.transform.position + this.posOffset; 
        this.posOffset -= new Vector3(1.2f,1.2f, 1.2f);

        // creating the nodes
        ClassVisualisation.MethodAndParameterNodes[] spellClass = this.classVisualisation.GenerateClassVisualisation(
            this.classVisualisation.GenerateNodeData(type), position, out Node node);
        
        // registering them for persistence 
        this.UI.connTracker.RegisterClassNameForPersistence(
            new ClassTracking {Name = type.FullName, node = node}, null);
    }

    private int id = 0;
    
    public int AddVariable(string varName)
    {
        id++;
        InputCanvas.InputElements variable = this.inputCanvas.CreateInputCanvas(default, id, this.UI, true, varName);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, varName, null), null);
        return id;
    }

    public void AddConstant(object value)
    {
        var contant = this.inputCanvas.CreateInputCanvas(value, ++id, this.UI, false);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, null, value), null);
    }

    #region REMOVE ELEMENTS
    
    public void UnregisterNode(Type type)
    {
        Node node = this.UI.connTracker.UnregisterClassName(type.FullName, null);
        this.UI.drawer.RemoveInterClassLine(node);
        GameObject.Destroy(node.gameObject);
    }
    
    public void UnregisterDirectInput(int id)
    {
        this.UI.connTracker.UnregisterDirectInput(id, null);
        ReferenceBuffer.Instance.worldSpaceUI.inputCanvas.RemoveInputCanvas(id);
    }

    #endregion

}