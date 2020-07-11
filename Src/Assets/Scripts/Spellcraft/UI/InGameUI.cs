using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Material transperantMat;
    public GameObject worldSpaceCanvasPrefab;

    private Camera myCamera;
    private SpellcraftCam camHanding;
    private Node dragged = null;
    private LineDrawer drawer;
    private Defunclator classVisualisation;
    private GameObject text1;
    private TMP_Text worldSpaceText;
    private GameObject menu1;
    private GameObject result;
    private ConnectionsTracker tracker = new ConnectionsTracker();
    private ConstantElements[] constants;

    private void Start()
    {
        this.text1 = GameObject.Find("WSCText1");
        this.menu1 = GameObject.Find("WSCMenu1");
        this.worldSpaceText = this.text1.transform.Find("Text").GetComponent<TMP_Text>();

        this.DrawBox(SpellcraftConstants.HalfSize, SpellcraftConstants.Thickness, SpellcraftConstants.BoxCenter);
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();
        this.camHanding = this.myCamera.gameObject.AddComponent<SpellcraftCam>();
        this.camHanding.SetTarget(new GameObject("Center"));
        this.drawer = gameObject.GetComponent<LineDrawer>();
        this.classVisualisation = new Defunclator(this);

        this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Math>(), new Vector3(-4, 0, 0));
        this.classVisualisation.GenerateClassVisualisation(this.classVisualisation.GenerateNodeData<Math>(), new Vector3(+4, 0, 0));


        this.constants = new ConstantElements[]
        {
            this.CreateConstantCanvas(12),
            this.CreateConstantCanvas(13),
            this.CreateConstantCanvas(14),
            this.CreateConstantCanvas(15),
        };

        this.result = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.result.transform.position = new Vector3(0, -15, 0);
        this.result.AddComponent<ResultNode>().Setup(typeof(int), this);
    }

    public void RegisterConstantClick(ConstantNode node)
    {
        if (this.lastClickedParameter != null)
        {
            this.tracker.TrackParameterAssignConstant(this.lastClickedParameter, node);

            this.ConstantsHide();

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

        this.tracker.TrackResultAssignMethodCall(this.lastClickedMethod);

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

        this.ConstantsDisplay(node.transform.position);

        if (this.lastClickedMethod == null)
        {
            return;
        }

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

    public void SetWorldCanvasText(string text)
    {
        this.worldSpaceText.text = text;
    }

    public void SetWorldCanvasPosition(Vector3 position)
    {
        RectTransform rt = this.text1.GetComponent<RectTransform>();
        rt.position = position;
        rt.LookAt(rt.transform.position + this.myCamera.transform.forward);
    }

    public void ConstantsDisplay(Vector3 pos)
    {
        for (int i = 0; i < this.constants.Length; i++)
        {
            ConstantElements c = this.constants[i];

            if(c.Used == true)
            {
                return;
            }

            c.GameObject.SetActive(true);
            c.RectTransform.position = pos + new Vector3(1.5f,i,0); 
            c.RectTransform.LookAt(c.RectTransform.transform.position + this.myCamera.transform.forward);
        }
    }

    public void ConstantsHide()
    {
        for (int i = 0; i < this.constants.Length; i++)
        {
            ConstantElements c = this.constants[i];
            c.GameObject.SetActive(false);
        }
    }

    private void ClassTest()
    {
        this.classVisualisation.BuildUI(this.classVisualisation.GenerateNodeData<Some>());
    }

    public class Some
    {
        public int one { get; set; }

        public string two { get; set; }

        public int three { get; set; }

        public string four { get; set; }

        public int SomeOne(int one, int two, int three, int four)
        {
            return 0;
        }

        public int SomeTwo(int one, int two, int three, int four)
        {
            return 0;
        }

        public int SomeThree(int one, int two, int three, int four)
        {
            return 0;
        }

        public int SomeFour(int one, int two, int three, int four, int five)
        {
            return 0;
        }
    }

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

    private void DefaultInterface()
    {
        GameObject node = GameObject.CreatePrimitive(PrimitiveType.Cube);
        node.AddComponent<Node>().Setup(this);
        node.name = "node";
        GameObject node2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        node2.AddComponent<Node>().Setup(this);
        node2.name = "node";
        this.drawer.RegisterLine(node.transform, node2.transform, 0.2f, Color.cyan);
    }

    public void SetDragged(Node dragged)
    {
        this.dragged = dragged;
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

    private void Update()
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.camHanding.UntriggerZoom();
        }
    }

    private ConstantElements CreateConstantCanvas(int value)
    {
        GameObject obj = GameObject.Instantiate(this.worldSpaceCanvasPrefab);

        Canvas can = obj.GetComponent<Canvas>();

        can.worldCamera = this.myCamera;

        GameObject buttonGo = obj.transform.Find("Button").gameObject;

        GameObject textGO = buttonGo.transform.Find("Text").gameObject;

        TMP_Text text = textGO.GetComponent<TMP_Text>();

        text.text = value.ToString();

        ConstantNode nodeBe = buttonGo.AddComponent<ConstantNode>();

        nodeBe.Setup(value, this);

        RectTransform rt = obj.GetComponent<RectTransform>();

        ConstantElements result = new ConstantElements(obj, text, nodeBe, rt);

        return result;
    }

    public class ConstantElements
    {
        public GameObject GameObject { get; set; }

        public TMP_Text text { get; set; }

        public ConstantNode Node { get; set; }

        public RectTransform RectTransform { get; set; } 

        public bool Used { get; set; } = false;

        public ConstantElements(GameObject canvas, TMP_Text text, ConstantNode node,RectTransform rectTransform)
        {
            this.RectTransform = rectTransform;
            this.GameObject = canvas;
            this.text = text;
            this.Node = node;
        }
    }
}

