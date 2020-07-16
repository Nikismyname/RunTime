#region INIT

using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUI : MonoBehaviour
{
    public Material transperantMat;
    public GameObject constantAndVariablePanelPrefab;
    public GameObject resultAndVariablesPanelPrefab;

    public ZoomMode zoomMode { get; set; } = ZoomMode.OuterZoom;
    private Camera myCamera;
    private SpellcraftCam camHanding;
    private Node dragged = null;
    private LineDrawer drawer;
    private Defunclator classVisualisation;
    private GameObject text1;
    private TMP_Text worldSpaceText;
    private GameObject menu1;
    private GameObject result;
    private ConnectionsTracker connTracker = new ConnectionsTracker();
    private ConstantElements[] constants;
    private GameObject rotatorGO;
    private ObjectRotator objRotator;
    private Transform constantsParent;
    private float constantsScale = 0.3f;
    private bool constantsShowing = false;
    private ResultCanvas resultCanvas;
    private GameObject resultCanvasVantigePoint;
        
    private void Start()
    {
        this.constantsParent = new GameObject("Contants Parent!").transform;
        this.rotatorGO = new GameObject("Rotator");
        this.objRotator = this.rotatorGO.AddComponent<ObjectRotator>();

        this.text1 = GameObject.Find("WSCText1");
        this.menu1 = GameObject.Find("WSCMenu1");
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

        //this.JustAddMethod();
        this.JustTwoAddMethod();

        this.resultCanvasVantigePoint = new  GameObject("VantigePoint");
        this.resultCanvasVantigePoint.transform.position = new Vector3(0,0,-30);
    }

    #endregion

    #region LEVELS

    public void JustAddMethod()
    {
        ///CLASS NODES
        this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Test>(), new Vector3(0, 0, 0));

        ///CONSTANTS
        this.constants = new ConstantElements[]
        {
            this.CreateConstantCanvas(12, this.constantsParent),
            this.CreateConstantCanvas(13, this.constantsParent),
            this.CreateConstantCanvas(14, this.constantsParent),
            this.CreateConstantCanvas(15, this.constantsParent),
        };

        ///RESULT NODE
        this.result = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.result.transform.position = new Vector3(0, -15, 0);
        this.result.AddComponent<ResultNode>().Setup(typeof(int), this);
    }

    public void JustTwoAddMethod()
    {
        ///CLASS NODES
        this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Test>(), new Vector3(0, +5, 0));
        this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Test>(), new Vector3(0, -5, 0));

        ///CONSTANTS
        this.constants = new ConstantElements[]
        {
            this.CreateConstantCanvas(12, this.constantsParent),
            this.CreateConstantCanvas(13, this.constantsParent),
            this.CreateConstantCanvas(14, this.constantsParent),
            this.CreateConstantCanvas(15, this.constantsParent),
        };

        ///RESULT NODE
        this.result = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.result.transform.position = new Vector3(0, -15, 0);
        this.result.AddComponent<ResultNode>().Setup(typeof(int), this);

        this.resultCanvas.SetVariables(new ResultCanvas.VariableInput[]
        {
            new ResultCanvas.VariableInput(typeof(int),"int1WithExtras"),
            new ResultCanvas.VariableInput(typeof(int),"int2WithExtras")
        });
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
                this.ConstantsHide();
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

    #region CONSTANTS

    public async void ConstantsDisplay(Vector3 pos, ParameterNode node)
    {
        int columnCount = 2;
        float buttonX = 1.5f;
        float buttonY = 1f;
        /// Scaling
        buttonX *= this.constantsScale;
        buttonY *= this.constantsScale;
        ///...
        int rows = (int)Mathf.Ceil((float)this.constants.Length / columnCount);
        float wholeY = rows * buttonY;
        float wholeX = buttonX * columnCount;


        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columnCount; x++)
            {
                int index = y * columnCount + x;

                if (index >= this.constants.Length)
                {
                    break;
                }

                ConstantElements c = this.constants[index];

                c.ParentObject.SetActive(false);

                float yy = y * buttonY - wholeY / 2 + buttonY / 2;
                float xx = x * buttonX - wholeX / 2 + buttonX / 2;

                c.RectTransform.localPosition = new Vector3(xx, yy, 0);

                c.Node.FixColorBeforeShow(node);
            }
        }

        this.constantsParent.LookAt(this.constantsParent.transform.position + this.myCamera.transform.forward);

        Vector3 offset = (this.myCamera.transform.position - pos).normalized;

        this.constantsParent.position = pos + offset;

        this.constantsShowing = true;

        /// Enabaling the constant canvases after some time so one does not get automatically clicked
        await Task.Delay(100);

        foreach (var item in this.constants)
        {
            item.ParentObject.SetActive(true);
        }
    }

    public void ConstantsHide()
    {
        for (int i = 0; i < this.constants.Length; i++)
        {
            ConstantElements c = this.constants[i];
            c.ParentObject.SetActive(false);
        }

        this.constantsShowing = false;
    }

    private ConstantElements CreateConstantCanvas(int value, Transform parent)
    {
        GameObject obj = GameObject.Instantiate(this.constantAndVariablePanelPrefab);

        Canvas can = obj.GetComponent<Canvas>();

        can.worldCamera = this.myCamera;

        GameObject buttonGo = obj.transform.Find("Button").gameObject;

        Button button = buttonGo.GetComponent<Button>();

        GameObject textGO = buttonGo.transform.Find("Text").gameObject;

        TMP_Text text = textGO.GetComponent<TMP_Text>();

        text.text = value.ToString();

        ConstantNode nodeBe = buttonGo.AddComponent<ConstantNode>();

        RectTransform rt = obj.GetComponent<RectTransform>();

        ConstantElements result = new ConstantElements(obj, text, nodeBe, rt, button);

        obj.transform.SetParent(parent);

        ///element scaling!
        rt.localScale *= this.constantsScale;
        ///

        nodeBe.Setup(value, this, result);

        return result;
    }

    public class ConstantElements
    {
        public GameObject ParentObject { get; set; }

        public TMP_Text text { get; set; }

        public ConstantNode Node { get; set; }

        public RectTransform RectTransform { get; set; }

        public Button Button { get; set; }

        public bool Used { get; set; } = false;

        public ConstantElements(GameObject canvas, TMP_Text text, ConstantNode node, RectTransform rectTransform, Button button)
        {
            this.RectTransform = rectTransform;
            this.ParentObject = canvas;
            this.text = text;
            this.Node = node;
            this.Button = button;
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

    public void RegisterConstantClick(ConstantNode node)
    {
        if (this.lastClickedParameter != null)
        {
            if (node.elements.Used)
            {
                Debug.Log("constant already used");
                return;
            }

            this.connTracker.TrackParameterAssignConstant(this.lastClickedParameter, node);

            this.ConstantsHide();

            this.lastClickedParameter.RegisterAssignment();

            ConstantElements n = this.constants.Single(x => x.Node == node);

            n.Used = true;
            n.Node.SetUsed(true, this.lastClickedParameter);

            return;
        }

        if (this.lastClickedProperty != null)
        {
            ///TODO:
        }
    }

    public void RegisterResultClick(ResultNode resultNode)
    {
        if (this.lastClickedMethod == null)
        {
            return;
        }

        this.connTracker.TrackResultAssignMethodCall(this.lastClickedMethod);

        this.DrawConnection(this.result, this.lastClickedMethod.gameObject);

        this.lastClickedMethod = null;
    }

    private PropertyNode lastClickedProperty;

    public void RegisterPropertyClick(PropertyNode node)
    {
        this.lastClickedProperty = node;
        /// DO IT!
    }

    private ParameterNode lastClickedParameter = null;

    public void RegisterParameterClick(ParameterNode node)
    {
        this.lastClickedParameter = node;

        this.lastClickedParameter.RegisterSelection();

        this.ConstantsDisplay(node.transform.position, node);

        if (this.lastClickedMethod == null)
        {
            return;
        }

        this.connTracker.TrackParameterAssignMethod(node, this.lastClickedMethod);

        this.DrawConnection(node.gameObject, this.lastClickedMethod.gameObject);

        this.lastClickedParameter = null;

        this.lastClickedMethod = null;
    }

    private MethodNode lastClickedMethod = null;

    public void RegisterMethodClick(MethodNode node)
    {
        this.lastClickedMethod = node;

        if (this.lastClickedParameter == null)
        {
            return;
        }

        this.DrawConnection(node.gameObject, this.lastClickedParameter.gameObject);

        this.lastClickedParameter = null;
        this.lastClickedMethod = null;
    }

    #endregion

    #region }
}
#endregion

