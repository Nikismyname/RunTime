#region INIT

using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    public Material transperantMat;
    public GameObject inputPanelPrefab;
    public GameObject resultAndVariablesPanelPrefab;
    public GameObject worldSpaceTextPrefab;

    public ZoomMode zoomMode { get; set; } = ZoomMode.OuterZoom;
    public GameObject parent;
    private Camera myCamera;
    private SpellcraftCam camHanding;
    private Node dragged = null;
    private Defunclator classVisualisation;
    private GameObject resultGO;
    private ConnectionsTracker connTracker = new ConnectionsTracker();
    private GameObject rotatorGO;
    private ObjectRotator objRotator;
    private GameObject resultCanvasVantigePoint;

    public ConnectionsRegisterer connRegisterer;
    public InfoCanvas infoCanvas;
    private ResultCanvas resultCanvas;
    private InputCanvas inputCanvas;
    private LineDrawer drawer;
    private Levels levels;

    private void Start()
    {
        this.parent = new GameObject("Spellcraft Parent");
 
        ///Rotation of class nodes implementation that wull be replaced
        this.rotatorGO = new GameObject("Rotator");
        this.rotatorGO.transform.SetParent(this.parent.transform);
        this.objRotator = this.rotatorGO.AddComponent<ObjectRotator>();
        ///...
        
        ///Parented
        this.drawer = new LineDrawer(this);
        this.drawer.DrawBox(SpellcraftConstants.HalfSize, SpellcraftConstants.Thickness, SpellcraftConstants.BoxCenter);
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();
        this.camHanding = this.myCamera.gameObject.AddComponent<SpellcraftCam>();
        GameObject center = new GameObject("Center");
        center.transform.SetParent(this.parent.transform);
        this.camHanding.Setup(center);
        this.classVisualisation = new Defunclator(this);
        this.resultCanvas = new ResultCanvas(this.resultAndVariablesPanelPrefab, this.myCamera, this.connTracker, this.parent.transform);
        this.resultCanvas.SetPosition(new Vector3(0, 0, -25));
        this.resultCanvas.SetScale(new Vector3(0.02f, 0.02f, 0.02f));
        this.resultCanvas.Hide();
        this.inputCanvas = new InputCanvas(this.myCamera, this.inputPanelPrefab, this.parent.transform);
        
        ///Extract that somewhere
        this.resultGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.resultGO.name = "Result GO";
        this.resultGO.transform.SetParent(this.parent.transform);
        this.resultGO.transform.position = new Vector3(0, -15, 0);
        ResultNode resultNode = this.resultGO.AddComponent<ResultNode>();

        this.connRegisterer = new ConnectionsRegisterer(this.connTracker, this.inputCanvas, this.drawer, resultNode);
        this.infoCanvas = new InfoCanvas(this.worldSpaceTextPrefab, this.myCamera, this.parent.transform);
        this.levels = new Levels(this.resultCanvas, this.inputCanvas, this.connRegisterer, this, resultNode, this.classVisualisation);
        this.levels.JustTwoAddMethod(true);
        this.resultCanvasVantigePoint = new GameObject("VantigePoint");
        this.resultCanvasVantigePoint.transform.position = new Vector3(0, 0, -30);
        this.resultCanvasVantigePoint.transform.SetParent(this.parent.transform);
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
            this.connRegisterer.ResetToNull();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            this.resultCanvas.Show();
            this.camHanding.SetRotateToView(this.resultCanvasVantigePoint);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.inputCanvas.AreInputsShowing())
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

    private void DragControllsOnUpdate()
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

    #region PUBLIC_INTERFACE

    public void SetDragged(Node dragged)
    {
        this.dragged = dragged;
    }

    #endregion

    #region }
}
#endregion

