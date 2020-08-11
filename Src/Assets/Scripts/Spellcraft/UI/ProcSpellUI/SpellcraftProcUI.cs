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

    public List<ActionButtonMap> actionButtonData = new List<ActionButtonMap>();

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

        Vector2 tl = new Vector2(-this.canvasHalfX, this.canvasHalfY);
        
        DrawSavedCubesRow savedRow = new DrawSavedCubesRow(tl, Color.white, generator, this );
        savedRow.GenerateUI(out Vector2 offset1);
        tl += offset1;
        
        DrawSavingCube savingCube = new DrawSavingCube(tl, Color.white, generator, this);
        savingCube.GenerateUI(out Vector2 offset2); 
        tl += offset2;
        
        this.DrawSaveCubeMenu(tl, out float x2);
        tl += new Vector2(x2, 0);
        this.DrawActionButtonMapping(tl, out float x3);
        tl += new Vector2(x3, 0);
        this.DrawVariableManagement(tl, out float x4);
        tl += new Vector2(x4, 0);
        this.DrawTypesSelection(tl, out float _);;
    }

    #endregion

    public void SetCanvasPosition(Vector3 pos)
    {
        this.canvasGO.transform.position = pos;
    }

    #region HIGER_LEVEL UL MAKERS

    private void DrawActionButtonMapping(Vector2 tl, out float x)
    {
        foreach (GameObject t in this.actionMapButtons)
        {
            Destroy(t);
        }

        this.actionMapButtons = new List<GameObject>();
        this.actionButtonData = new List<ActionButtonMap>();

        ActionKeyPersistance.ActionKeyPersistanceData[] mappings = ActionKeyPersistance.GetKeyCubeMapping();

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                GameObject mapButton = DrawButton("",
                    new Vector2(tl.x + this.xOffset + xx * (this.xOffset + this.buttonPixelsX),
                        tl.y - (yy * (this.yOffset + this.buttonPixelsY))));
                this.actionMapButtons.Add(mapButton);
                int keyId = yy * 3 + xx + 1;
                ActionButtonMap node = new ActionButtonMap(keyId, mapButton, this.actionButtonData);
                ActionKeyPersistance.ActionKeyPersistanceData mapping = mappings.FirstOrDefault(y => y.KeyId == keyId);

                if (mapping != null)
                {
                    node.SetName(mapping.CubeName);
                }

                this.actionButtonData.Add(node);
                mapButton.GetComponent<Button>().onClick.AddListener(() => node.Select());
            }
        }

        x = (this.xOffset + this.buttonPixelsX) * 3;
    }

    private void DrawSaveCubeMenu(Vector2 tl, out float x)
    {
        foreach (GameObject t in this.saveButtons)
        {
            Destroy(t);
        }

        this.saveButtons = new List<GameObject>();

        GameObject text = this.DrawText(new Vector2(tl.x, tl.y), "Name The Save", 20);
        GameObject input = this.DrawInputMenu(new Vector2(tl.x, tl.y - this.yOffset - this.buttonPixelsY));
        GameObject saveButton = DrawButton("Save", new Vector2(tl.x, tl.y - (this.yOffset + this.buttonPixelsY) * 2));
        saveButton.GetComponent<Button>().onClick
            .AddListener(() => this.OnClickSave(input.GetComponent<TMP_InputField>()));
        this.saveButtons.Add(text);
        this.saveButtons.Add(input);
        this.saveButtons.Add(saveButton);

        x = this.buttonPixelsX + this.yOffset;
    }

    private void DrawVariableManagement(Vector2 tl, out float x)
    {
        foreach (GameObject t in this.variableMapping)
        {
            Destroy(t);
        }

        this.variableMapping = new List<GameObject>();

        string[] variableNames =
        {
            ResultCanvas.PlayerMarkerVarName, ResultCanvas.PlayerForwardVarName, ResultCanvas.DroneMarkerVarName,
            ResultCanvas.PlayerPositionVarName
        };

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                int index = yy * 3 + xx;

                if (index >= variableNames.Length)
                {
                    goto label;
                }

                GameObject variableButton = DrawButton(variableNames[index],
                    new Vector2(tl.x + this.xOffset + xx * (this.xOffset + this.buttonPixelsX),
                        tl.y - (yy * (this.yOffset + this.buttonPixelsY))));
                this.variableMapping.Add(variableButton);

                variableButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.AddVariable(variableNames[index]);
                });
            }
        }

        label:

        x = (this.xOffset + this.buttonPixelsX) * 3;
    }

    private void DrawTypesSelection(Vector2 tl, out float x)
    {
        foreach (GameObject t in this.typesSelection)
        {
            Destroy(t);
        }

        this.typesSelection = new List<GameObject>();

        string[] typeNames = DynamicSetup.Types.Select(z => z.Name).ToArray();

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                int index = yy * 3 + xx;

                if (index >= typeNames.Length)
                {
                    goto label;
                }

                GameObject variableButton = DrawButton(typeNames[index],
                    new Vector2(tl.x + this.xOffset + xx * (this.xOffset + this.buttonPixelsX),
                        tl.y - (yy * (this.yOffset + this.buttonPixelsY))));
                this.typesSelection.Add(variableButton);

                variableButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.RegisterNode<int>(DynamicSetup.Types[index]);
                });
            }
        }

        label:

        x = (this.xOffset + this.buttonPixelsX) * 3;
    }

    #endregion

    #region DATA_CLASSES

    public class ActionButtonMap
    {
        public ActionButtonMap(int id, GameObject gameObject, List<ActionButtonMap> all)
        {
            this.Id = id;
            this.GameObject = gameObject;
            this.all = all;
        }

        private List<ActionButtonMap> all;

        public int Id { get; }

        public GameObject GameObject { get; }

        public string Name { get; set; }

        public bool Selected { get; set; } = false;


        public void SetName(string name)
        {
            this.Name = name;

            this.GameObject.transform.Find("Text").GetComponent<TMP_Text>().text = name;
        }

        public void Select()
        {
            foreach (var item in this.all)
            {
                item.Deselect();
            }

            this.Selected = true;
            this.GameObject.GetComponent<Image>().color = Color.green;
        }

        public void Deselect()
        {
            this.Selected = false;
            this.GameObject.GetComponent<Image>().color = Color.white;
        }
    }

    #endregion
}