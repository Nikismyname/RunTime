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

    public DrawActionButtonMapping drawActionButtonMapping;
    public DrawSavedCubesRow drawSavedCubesRow;
    public DrawSavingCube drawSavingCube;
    public DrawVariableSelection drawVariableSelection;
    public DrawTypesSelection drawTypesSelection;

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
        drawActionButtonMapping = new DrawActionButtonMapping(Color.white, generator, this);
        drawSavedCubesRow = new DrawSavedCubesRow(Color.white, generator, this);
        drawSavingCube = new DrawSavingCube(Color.white, generator, this);
        drawVariableSelection = new DrawVariableSelection(Color.white, generator, this);
        drawTypesSelection = new DrawTypesSelection(Color.white, generator, this);

        Vector2 tl = new Vector2(-this.canvasHalfX, this.canvasHalfY);

        float interBlockOffset = this.xOffset * 4;
        
        drawSavedCubesRow.GenerateUI(tl, out Vector2 offset1);
        tl.x += offset1.x + interBlockOffset;

        drawSavingCube.GenerateUI(tl, out Vector2 offset2);
        tl.x += offset2.x + interBlockOffset;

        drawActionButtonMapping.GenerateUI(tl, out Vector2 offset3);
        tl.x += offset3.x + interBlockOffset;

        drawVariableSelection.GenerateUI(tl, out Vector2 offset4);
        tl.x += offset4.x + interBlockOffset;

        drawTypesSelection.GenerateUI(tl, out Vector2 offset5);
        tl.x += offset5.x + interBlockOffset;
    }

    #endregion

    public void SetCanvasPosition(Vector3 pos)
    {
        this.canvasGO.transform.position = pos;
    }
}