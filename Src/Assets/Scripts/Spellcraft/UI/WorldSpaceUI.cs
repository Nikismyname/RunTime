#region INIT

using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    public Material transperantMat;
    public GameObject inputPanelPrefab;
    public GameObject resultAndVariablesPanelPrefab;

    public ZoomMode zoomMode { get; set; } = ZoomMode.OuterZoom;
    private Camera myCamera;
    private SpellcraftCam camHanding;
    private Node dragged = null;
    private LineDrawer drawer;
    private Defunclator classVisualisation;
    private GameObject text1;
    private TMP_Text worldSpaceText;
    private GameObject resultGO;
    private ConnectionsTracker connTracker = new ConnectionsTracker();
    private GameObject rotatorGO;
    private ObjectRotator objRotator;
    private bool constantsShowing = false;
    private GameObject resultCanvasVantigePoint;

    private ResultCanvas resultCanvas;
    private InputCanvas inputCanvas;


    private void Start()
    {
        this.rotatorGO = new GameObject("Rotator");
        this.objRotator = this.rotatorGO.AddComponent<ObjectRotator>();

        this.text1 = GameObject.Find("WSCText1");
        this.worldSpaceText = this.text1.transform.Find("Text").GetComponent<TMP_Text>();

        this.DrawBox(SpellcraftConstants.HalfSize, SpellcraftConstants.Thickness, SpellcraftConstants.BoxCenter);
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();
        this.camHanding = this.myCamera.gameObject.AddComponent<SpellcraftCam>();
        this.camHanding.Setup(new GameObject("Center"));
        this.drawer = gameObject.GetComponent<LineDrawer>();
        this.classVisualisation = new Defunclator(this);

        this.resultCanvas = new ResultCanvas(this.resultAndVariablesPanelPrefab, this.myCamera, this.connTracker);
        this.resultCanvas.SetPosition(new Vector3(0, 0, -25));
        this.resultCanvas.SetScale(new Vector3(0.02f, 0.02f, 0.02f));

        this.inputCanvas = new InputCanvas(this.myCamera, this.inputPanelPrefab);

        this.JustTwoAddMethod(true);

        this.resultCanvasVantigePoint = new GameObject("VantigePoint");
        this.resultCanvasVantigePoint.transform.position = new Vector3(0, 0, -30);
    }

    #endregion

    #region LEVELS

    public async void JustTwoAddMethod(bool solved = false)
    {
        //var thing = this.classVisualisation.GenerateNodeData<Test>();

        ///CLASS NODES
        var nodes1 = this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Test>(), new Vector3(0, +5, 0));
        var nodes2 = this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Test>(), new Vector3(0, -5, 0));

        ///RESULT NODE
        this.resultGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.resultGO.transform.position = new Vector3(0, -15, 0);
        var resultNode = this.resultGO.AddComponent<ResultNode>();
        resultNode.Setup(typeof(int), this);

        //TODO: The names are truncated for the frontend and names need to be 4 sumbols for things to match. FIX IT!
        ResultCanvas.VariableInput var1 = new ResultCanvas.VariableInput(typeof(int), "int1");
        ResultCanvas.VariableInput var2 = new ResultCanvas.VariableInput(typeof(int), "int2");

        this.resultCanvas.SetVariables(new ResultCanvas.VariableInput[]
        {
            var1,
            var2,
        });

        var contant1 = this.inputCanvas.CreateInputCanvas(12, this, false);
        var contant2 = this.inputCanvas.CreateInputCanvas(13, this, false);
        var contant3 = this.inputCanvas.CreateInputCanvas(14, this, false);
        var contant4 = this.inputCanvas.CreateInputCanvas(15, this, false);
        var variable1 = this.inputCanvas.CreateInputCanvas(default, this, true, var1.Name);
        var variable2 = this.inputCanvas.CreateInputCanvas(default, this, true, var2.Name);

        if (solved)
        {
            await this.RegisterParameterClick(nodes1[0].Parameters[0], nodes2[0].Method);
            this.RegisterConstantClick(variable1.Node, nodes1[0].Parameters[1]);
            this.RegisterConstantClick(variable2.Node, nodes2[0].Parameters[0]);
            this.RegisterConstantClick(contant4.Node, nodes2[0].Parameters[1]);
            this.RegisterResultClick(resultNode, nodes1[0].Method);

            this.inputCanvas.InputsHide();
        }
    }

    #endregion

    #region CLASSES_TO_PARSE

    public class Math
    {
        public int Sum(int one, int two)
        {
            return one + two;
        }
        public int Subract(int one, int two)
        {
            return one - two;
        }

        public int Multiply(int one, int two)
        {
            return one * two;
        }

        public int Divide(int one, int two)
        {
            return one / two;
        }
    }

    public class Test
    {
        public int Sum(int one, int two)
        {
            return one + two;
        }
    }

    #endregion

    #region UPDATE

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.inputCanvas.InputsHide();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.lastClickedMethod = null;
            this.lastClickedParameter = null;
            this.lastClickedProperty = null;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            //this.connTracker.PrintResult();
            this.camHanding.SetRotateToView(this.resultCanvasVantigePoint);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.constantsShowing)
            {
                this.inputCanvas.InputsHide();
            }
            else
            {
                this.camHanding.UntriggerZoom();
                this.zoomMode = ZoomMode.OuterZoom;
            }
        }

        this.DragControllsOnUpdate();
    }

    public void DragControllsOnUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            this.dragged?.SetRotating(true);
            this.camHanding.GetMouseButtonDownOne();
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.camHanding.GetMouseButtonUpOne();

            if (this.dragged != null)
            {
                Vector3 thing = this.myCamera.WorldToScreenPoint(dragged.gameObject.transform.position);

            }

            this.dragged?.SetRotating(false);
        }
    }

    #endregion

    #region DRAW_WORLD

    private void DrawBox(float halfSize, float thickness, Vector3 center)
    {
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, -halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, halfSize, -halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness);
    }

    private GameObject DrawInGameLine(Vector3 from, Vector3 to, Color color, float thickness)
    {
        GameObject parent = new GameObject("LineParent");
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        parent.transform.position = new Vector3(0, -1, 0);
        line.transform.parent = parent.transform;

        parent.transform.position = from;
        parent.transform.LookAt(to);
        parent.transform.Rotate(new Vector3(1, 0, 0), 90);
        line.GetComponent<Renderer>().material.color = color;
        line.SetShader();
        parent.SetScale(new Vector3(thickness, (from - to).magnitude / 2, thickness));
        return parent;
    }

    private void DrawConnection(GameObject one, GameObject two)
    {
        if (one.transform.parent == two.transform.parent)
        {
            this.drawer.RegisterCurve(one.transform, two.transform, two.transform.parent.transform.position, 0.1f, Color.cyan, 1);
        }
        else
        {
            this.drawer.RegisterLine(one.transform, two.transform, 0.1f, Color.green);
        }
    }

    #endregion

    #region PUBLIC_INTERFACE

    public void SetTextWorldCanvasText(string text)
    {
        this.worldSpaceText.text = text;
    }

    public void SetTextWorldCanvasPosition(Vector3 position)
    {
        RectTransform rt = this.text1.GetComponent<RectTransform>();
        rt.position = position;
        rt.LookAt(rt.transform.position + this.myCamera.transform.forward);
    }

    public void SetDragged(Node dragged)
    {
        this.dragged = dragged;
    }

    #endregion

    #region PUBLIC_INTERFACE_REGISTER_NODE_CLICK

    public void RegisterConstantClick(ConstantNode node, ParameterNode paraNodeIn = null)
    {
        var paraNode = this.lastClickedParameter;
        if (paraNodeIn != null)
        {
            paraNode = paraNodeIn;
        }

        if (paraNode != null)
        {
            if (node.elements.Used)
            {
                Debug.Log("constant already used");
                return;
            }

            this.connTracker.TrackParameterAssignConstant(paraNode, node);

            this.inputCanvas.InputsHide();

            paraNode.RegisterAssignment();

            InputCanvas.InputElements n = this.inputCanvas.GetInputs().Single(x => x.Node == node);

            n.Used = true;
            n.Node.SetUsed(true, paraNode);

            return;
        }

        if (this.lastClickedProperty != null)
        {
            ///TODO:
        }
    }

    public void RegisterResultClick(ResultNode resultNode, MethodNode methodNodeIn = null)
    {
        var methodNode = this.lastClickedMethod;
        if (methodNodeIn != null)
        {
            methodNode = methodNodeIn;
        }

        if (methodNode == null)
        {
            return;
        }

        this.connTracker.TrackResultAssignMethodCall(methodNode);

        this.DrawConnection(this.resultGO, methodNode.gameObject);

        this.lastClickedMethod = null;
    }

    private PropertyNode lastClickedProperty;

    public void RegisterPropertyClick(PropertyNode node)
    {
        this.lastClickedProperty = node;
        /// DO IT!
    }

    private ParameterNode lastClickedParameter = null;

    public async Task RegisterParameterClick(ParameterNode node, MethodNode methodNodeIn = null)
    {
        this.lastClickedParameter = node;

        this.lastClickedParameter.RegisterSelection();

        await this.inputCanvas.InputsDisplay(node.transform.position, node);

        var methodNode = this.lastClickedMethod;
        if (methodNodeIn != null)
        {
            methodNode = methodNodeIn;
        }

        if (methodNode == null)
        {
            return;
        }

        this.connTracker.TrackParameterAssignMethod(node, methodNode);

        this.DrawConnection(node.gameObject, methodNode.gameObject);

        this.lastClickedParameter = null;

        this.lastClickedMethod = null;
    }

    private MethodNode lastClickedMethod = null;

    public void RegisterMethodClick(MethodNode node, ParameterNode paramNodeIn = null)
    {
        this.lastClickedMethod = node;

        var paramNode = this.lastClickedParameter;
        if(paramNodeIn != null)
        {
            paramNode = paramNodeIn;
        }

        if (paramNode == null)
        {
            return;
        }

        this.DrawConnection(node.gameObject, paramNode.gameObject);

        this.lastClickedParameter = null;
        this.lastClickedMethod = null;
    }

    #endregion

    #region }
}
#endregion

