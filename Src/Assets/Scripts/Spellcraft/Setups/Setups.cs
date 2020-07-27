#region 

using Boo.Lang;
using UnityEngine;

public class Setups
{
    private ResultCanvas resultCanvas;
    private InputCanvas inputCanvas;
    private ConnectionsRegisterer connRegisterer;
    private WorldSpaceUI UI;
    private ResultNode resultNode;
    private ClassVisualisation classVisualisation;
    private List<ClassVisualisation.MethodAndParameterNodes[]> customNodes = new List<ClassVisualisation.MethodAndParameterNodes[]>();


    public Setups(ResultCanvas resultCanvas, InputCanvas inputCanvas, ConnectionsRegisterer connRegisterer, WorldSpaceUI UI, ResultNode resultNode, ClassVisualisation classVisualisation)
    {
        this.resultCanvas = resultCanvas;
        this.inputCanvas = inputCanvas;
        this.connRegisterer = connRegisterer;
        this.UI = UI;
        this.classVisualisation = classVisualisation;
        this.resultNode = resultNode;
        ///Setup the result!
        this.resultNode.Setup(typeof(int), this.UI);
    }

    #endregion

    #region PRESETS

    public async void JustTwoAddMethod(bool solved = false)
    {
        ///CLASS NODES
        var nodes1 = this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<TestClasses.Test>(), new Vector3(0, +5, 0), out _);
        //this.UI.connTracker.RegisterClassName(nameof(TestClasses.Test));
        var nodes2 = this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<TestClasses.Test>(), new Vector3(0, -5, 0), out _);
        //this.UI.connTracker.RegisterClassName(nameof(TestClasses.Test));

        //TODO: The names are truncated for the frontend and names need to be 4 sumbols for things to match. FIX IT!
        ResultCanvas.VariableInput var1 = new ResultCanvas.VariableInput(typeof(int), "addOne");
        ResultCanvas.VariableInput var2 = new ResultCanvas.VariableInput(typeof(int), "addTwo");

        this.resultCanvas.SetVariables(new ResultCanvas.VariableInput[]
        {
            var1,
            var2,
        });

        int id = 0;

        var contant1 = this.inputCanvas.CreateInputCanvas(12, id++, this.UI, false);
        var contant2 = this.inputCanvas.CreateInputCanvas(13, id++, this.UI, false);
        var contant3 = this.inputCanvas.CreateInputCanvas(14, id++, this.UI, false);
        var contant4 = this.inputCanvas.CreateInputCanvas(15, id++, this.UI, false);
        var variable1 = this.inputCanvas.CreateInputCanvas(default, id++, this.UI, true, var1.Name);
        var variable2 = this.inputCanvas.CreateInputCanvas(default, id++, this.UI, true, var2.Name);

        if (solved)
        {
            await this.connRegisterer.RegisterParameterClick(nodes1[0].Parameters[0], nodes2[0].Method);
            this.connRegisterer.RegisterConstantClick(variable1.Node, nodes1[0].Parameters[1]);
            this.connRegisterer.RegisterConstantClick(variable2.Node, nodes2[0].Parameters[0]);
            this.connRegisterer.RegisterConstantClick(contant4.Node, nodes2[0].Parameters[1]);
            this.connRegisterer.RegisterResultClick(this.resultNode, nodes1[0].Method);

            this.inputCanvas.InputsHide();
        }
    }

    #endregion

    #region SPELLCRAFT_TEST

    public void SpellCraft_TEST(bool solved = false)
    {
        this.UI.connTracker.RegisterBundle(null);

        ///CLASS NODES
        var spellClass = this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<SpellcraftClasses.Projectile>(), new Vector3(0, 0, 0), out Node one);
        this.UI.connTracker.RegisterClassNameForPersistance(new ClassTracking { Name = typeof(SpellcraftClasses.Projectile).FullName, node = one }, null);

        var vecUtil = this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Vector3Classes.Vector3Util>(), new Vector3(0, +5, 0), out Node two);
        this.UI.connTracker.RegisterClassNameForPersistance(new ClassTracking { Name = typeof(Vector3Classes.Vector3Util).FullName, node = two }, null);

        //TODO: The names are truncated for the frontend and names need to be 6 sumbols for things to match. FIX IT!
        ResultCanvas.VariableInput var1 = new ResultCanvas.VariableInput(typeof(Vector3), ResultCanvas.PlayerPositionVarName);
        ResultCanvas.VariableInput var2 = new ResultCanvas.VariableInput(typeof(Vector3), ResultCanvas.PlayerForwardVarName);

        this.resultCanvas.SetVariables(new ResultCanvas.VariableInput[]
        {
            var1,
            var2,
        });

        int id = 0;

        var contant1 = this.inputCanvas.CreateInputCanvas(0.5f, ++id, this.UI, false);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, null, 0.5f), null);
        var contant2 = this.inputCanvas.CreateInputCanvas(0.5f, ++id, this.UI, false);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, null, 0.5f), null);
        var contant3 = this.inputCanvas.CreateInputCanvas(14, ++id, this.UI, false);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, null, 14), null);
        var contant4 = this.inputCanvas.CreateInputCanvas(15, ++id, this.UI, false);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, null, 15), null);
        var contant5 = this.inputCanvas.CreateInputCanvas(16, ++id, this.UI, false);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, null, 16), null);
        var variable1 = this.inputCanvas.CreateInputCanvas(default, ++id, this.UI, true, ResultCanvas.PlayerPositionVarName);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, ResultCanvas.PlayerPositionVarName, null), null);
        var variable2 = this.inputCanvas.CreateInputCanvas(default, ++id, this.UI, true, ResultCanvas.PlayerForwardVarName);
        this.UI.connTracker.RegisterDirectInput(new DirectInput(id, ResultCanvas.PlayerForwardVarName, null), null);
    }

    #endregion

    #region CUSTOM_NOT_DONE

    //public ClassVisualisation.MethodAndParameterNodes[] AddToCustomSpellcraft<T>() where T : class, new()
    //{
    //    var nodes = this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<T>(), new Vector3(0, 0, 0));
    //    this.customNodes.Add(nodes);
    //    return nodes;
    //}

    public void CustomSpellcraft()
    {

    }

    #endregion
}
