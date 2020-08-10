﻿#region INIT

using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    public Material transperantMat;
    public GameObject inputPanelPrefab;
    public GameObject resultAndVariablesPanelPrefab;
    public GameObject worldSpaceTextPrefab;
    public bool LoadLevel;

    public ZoomMode zoomMode { get; set; } = ZoomMode.OuterZoom;
    public GameObject levelSpecificParent;
    public GameObject persistantParent;
    private Camera myCamera;
    private Camera myCamera2;
    private SpellMenuDragCam camHandling2;
    private SpellcraftCam camHandling;
    private Node dragged = null;
    private GameObject resultGO;
    private GameObject rotatorGO;
    private GameObject resultCanvasVantigePoint;

    public ClassVisualisation classVisualisation;
    public LineDrawer drawer;
    public Setups levels;
    public ConnectionsRegisterer connRegisterer;

    public ConnectionsTracker connTracker;
    public ResultCanvas resultCanvas;
    public InputCanvas inputCanvas;
    public InfoCanvas infoCanvas;
    public SpellcraftProcUI procUI;
    public GameObject procUIAnchor;
    public DynamicSetup dynamicSetup;

    public ResultNode resultNode;
    private Vector3 position;

    private void Start()
    {
        if (transperantMat == null)
        {
            this.transperantMat = Resources.Load("Materials/transperantMat", typeof(Material)) as Material;
            this.inputPanelPrefab = Resources.Load("Prefabs/WorldSpaceCanvases/ConstantCanvasPrefab", typeof(GameObject)) as GameObject;
            this.resultAndVariablesPanelPrefab = Resources.Load("Prefabs/WorldSpaceCanvases/ResultCanvasPrefab", typeof(GameObject)) as GameObject;
            this.worldSpaceTextPrefab = Resources.Load("Prefabs/WorldSpaceCanvases/WorldSpaceTextPrefab", typeof(GameObject)) as GameObject;
        }

        this.levelSpecificParent = new GameObject("Spellcraft Parent");
        this.persistantParent = new GameObject("Persistant Parent");

        this.connTracker = new ConnectionsTracker(this);

        ///Rotation of class nodes implementation that wull be replaced
        this.rotatorGO = new GameObject("Rotator");
        this.rotatorGO.transform.SetParent(this.levelSpecificParent.transform);
        ///...

        ///Parented
        this.drawer = new LineDrawer(this);
        this.drawer.DrawBox(SpellcraftConstants.HalfSize, SpellcraftConstants.Thickness, SpellcraftConstants.BoxCenter);
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();
        this.camHandling = this.myCamera.gameObject.AddComponent<SpellcraftCam>();
        this.myCamera2 = GameObject.Find("Camera2").GetComponent<Camera>();
        this.camHandling2 = this.myCamera2.gameObject.AddComponent<SpellMenuDragCam>();
        this.myCamera2.enabled = false;

        this.procUI = this.GetComponent<SpellcraftProcUI>();
        this.procUI.Setup(this.myCamera2, this.connTracker, 0.035f);
        float YY = 40;
        this.procUI.SetCanvasPosition(new Vector3(20, YY, 20));
        this.procUIAnchor = new GameObject("ProcUIAnchor");
        this.procUIAnchor.transform.position = new Vector3(20, YY, 0);

        GameObject center = new GameObject("Center");
        center.transform.SetParent(this.levelSpecificParent.transform);
        this.camHandling.Setup(center);
        this.classVisualisation = new ClassVisualisation(this);
        this.resultCanvas = new ResultCanvas(this.resultAndVariablesPanelPrefab, this.myCamera, this.connTracker, this.persistantParent.transform);
        this.resultCanvas.SetPosition(new Vector3(0, 0, -25));
        this.resultCanvas.SetScale(new Vector3(0.02f, 0.02f, 0.02f));
        this.resultCanvas.Hide();
        this.inputCanvas = new InputCanvas(this.myCamera, this.inputPanelPrefab, this.persistantParent.transform);

        ///Extract that somewhere
        this.resultGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.resultGO.name = "Result GO";
        this.resultGO.transform.SetParent(this.persistantParent.transform);
        this.resultGO.transform.position = new Vector3(0, -15, 0);
        this.resultNode = this.resultGO.AddComponent<ResultNode>();
        ///... 

        this.connRegisterer = new ConnectionsRegisterer(this.connTracker, this.inputCanvas, this.drawer, this.resultNode);
        this.infoCanvas = new InfoCanvas(this.worldSpaceTextPrefab, this.myCamera, this.levelSpecificParent.transform);
        this.levels = new Setups(this.resultCanvas, this.inputCanvas, this.connRegisterer, this, this.resultNode, this.classVisualisation);
        this.resultCanvasVantigePoint = new GameObject("VantigePoint");
        this.resultCanvasVantigePoint.transform.position = new Vector3(0, 0, -30);
        this.resultCanvasVantigePoint.transform.SetParent(this.levelSpecificParent.transform);
        
        this.dynamicSetup = new DynamicSetup(this.classVisualisation, this, this.resultCanvas, this.inputCanvas); 

        //this.levels.JustTwoAddMethod(true);
        if (LoadLevel)
        {
            //this.levels.LineDestroyer(false);
        }

        this.levelSpecificParent.transform.position = this.position;
        this.persistantParent.transform.position = this.position;
    }

    private void Reinit()
    {
        this.levelSpecificParent = new GameObject("Spellcraft Parent");

        ///Rotation of class nodes implementation that wull be replaced
        this.rotatorGO = new GameObject("Rotator");
        this.rotatorGO.transform.SetParent(this.levelSpecificParent.transform);
        ///...

        this.drawer.DrawBox(SpellcraftConstants.HalfSize, SpellcraftConstants.Thickness, SpellcraftConstants.BoxCenter);

        GameObject center = new GameObject("Center");
        center.transform.SetParent(this.levelSpecificParent.transform);
        this.camHandling.Setup(center);

        this.resultCanvasVantigePoint = new GameObject("VantigePoint");
        this.resultCanvasVantigePoint.transform.position = new Vector3(0, 0, -30);
        this.resultCanvasVantigePoint.transform.SetParent(this.levelSpecificParent.transform);
    }

    public void Setup(Vector3 position)
    {
        this.position = position;
    }

    #endregion

    #region UPDATE

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            this.connTracker.Persist();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            this.ResetData();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            this.inputCanvas.InputsHide();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.connRegisterer.ResetToNull();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.resultCanvas.Show();
            this.camHandling.SetRotateToView(this.resultCanvasVantigePoint);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.inputCanvas.AreInputsShowing())
            {
                this.inputCanvas.InputsHide();
            }
            else
            {
                this.camHandling.UntriggerZoom();
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
            this.camHandling.GetMouseButtonDownOne();
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.camHandling.GetMouseButtonUpOne();

            if (this.dragged != null)
            {
                Vector3 thing = this.myCamera.WorldToScreenPoint(dragged.gameObject.transform.position);

            }

            this.dragged?.SetRotating(false);
        }
    }

    public void SwitchToMenu()
    {
        /// SECOND CAM IS ON
        if (this.myCamera.enabled == false)
        {
            this.camHandling.SetEnabled(true);
            this.camHandling2.UnsetTarget(this.myCamera.gameObject);
            //Debug.Log("SPELL MOVE TO MAILN");
        }
        /// FIRST CAM IS ON
        else
        {
            this.camHandling.SetEnabled(false);
            this.myCamera2.enabled = true;
            this.myCamera.enabled = false;
            this.camHandling2.SetTarget(this.procUIAnchor.transform.position, this.myCamera.gameObject.transform.rotation, this.myCamera.gameObject.transform.position);
            //Debug.Log("SPELL MOVE TO second");
        }
    }

    #endregion

    #region PUBLIC_INTERFACE

    public void SetDragged(Node dragged)
    {
        this.dragged = dragged;
    }

    public void ResetData()
    {
        /// I was implementing the reset of all systems but then tought - maybe just destroy the parent and reinit what is needs reiniting!
        //this.infoCanvas.Reset();
        //this.connTracker.Reset();
        //this.resultCanvas.Reset(); ///Not implemented right now!
        //this.inputCanvas.Reset();

        Destroy(this.levelSpecificParent);
        ///
        this.Reinit();
        /// Resets the ids for generated nodes to 0 so it matches with the persisted values!
        this.classVisualisation.Reset();
        /// Removes all connection data for the previous level!
        this.connTracker.Reset();
        /// destroys the old input canvases and reinits the inputs List; 
        this.inputCanvas.Reset();
    }

    #endregion

    #region }
}
#endregion

