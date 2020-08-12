using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellcraftProcUI : MonoBehaviour
{
    #region INIT

    public GameObject buttonPrefab;
    public GameObject inputFieldPrefab;
    public GameObject dropDownPrefab;
    public GameObject textPrefab;
    public ConnectionsTracker tracker;
    private Camera myCamera;

    private float canvasHalfY = 250;
    private float canvasHalfX = 500;
    public int buttonPixelsX = 200;
    public int buttonPixelsY = 30;
    public float yOffset = 5f;
    public float xOffset = 5f;

    private float globalScale = 0.2f;

    public GameObject canvasGO;
    private Canvas canvas;

    //private List<GameObject> UIElements = new List<GameObject>();

    private List<GameObject> loadButtons = new List<GameObject>();
    private List<GameObject> saveButtons = new List<GameObject>();
    private List<GameObject> actionMapButtons = new List<GameObject>();
    private List<GameObject> variableMapping = new List<GameObject>();
    private List<GameObject> typesSelection = new List<GameObject>();

    public GameObject GetGameObject()
    {
        return this.canvasGO;
    }

    public void Setup(Camera cameraIn, ConnectionsTracker trackerIn, float globalScaleIn = 0.2f)
    {
        this.tracker = trackerIn;
        this.myCamera = cameraIn;
        this.globalScale = globalScaleIn;

        if (this.buttonPrefab == null)
        {
            this.buttonPrefab = Resources.Load("Prefabs/NonCanvases/TMP/TMPButton", typeof(GameObject)) as GameObject;
            this.inputFieldPrefab =
                Resources.Load("Prefabs/NonCanvases/TMP/TMPInputField", typeof(GameObject)) as GameObject;
            this.dropDownPrefab =
                Resources.Load("Prefabs/NonCanvases/TMP/TMPDropdown", typeof(GameObject)) as GameObject;
            this.textPrefab = Resources.Load("Prefabs/NonCanvases/TMP/TMPText", typeof(GameObject)) as GameObject;
        }

        this.canvasGO = new GameObject("MainCanvas");
        this.canvas = canvasGO.AddComponent<Canvas>();
        this.canvas.worldCamera = this.myCamera;
        var rectT = canvas.GetComponent<RectTransform>();
        this.canvasGO.AddComponent<CanvasScaler>();
        this.canvasGO.AddComponent<GraphicRaycaster>();
        rectT.sizeDelta = new Vector2(this.canvasHalfX * 2, this.canvasHalfY * 2);
        
        GenerateBasicElements generator = new GenerateBasicElements(this);
        DrawActionButtonMapping actionMaping = new DrawActionButtonMapping(Color.white, generator, this);
        DrawSavedCubesRow savedRow = new DrawSavedCubesRow( Color.white, generator, this, actionMaping);
        DrawSavingCube savingCube = new DrawSavingCube( Color.white, generator, this);
        DrawVariableSelection variableSelector = new DrawVariableSelection( Color.white, generator, this);
        DrawTypesSelection typeSelection = new DrawTypesSelection( Color.white, generator, this);
        
        Vector2 tl = new Vector2(-this.canvasHalfX, this.canvasHalfY);
        
        savedRow.GenerateUI(tl, out Vector2 offset1);
        tl.x += offset1.x;
        
        savingCube.GenerateUI(tl, out Vector2 offset2); 
        tl.x += offset2.x;
        
        actionMaping.GenerateUI(tl, out Vector2 offset3);
        tl.x += offset3.x;
        
        variableSelector.GenerateUI(tl, out Vector2 offset4);
        tl.x += offset4.x;

        typeSelection.GenerateUI(tl, out Vector2 offset5);
        tl.x += offset5.x;
    }

    #endregion

    public void SetCanvasPosition(Vector3 pos)
    {
        this.canvasGO.transform.position = pos;
    }
}