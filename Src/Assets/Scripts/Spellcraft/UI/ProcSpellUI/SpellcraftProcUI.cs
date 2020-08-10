﻿#region INIT

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellcraftProcUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject inputFieldPrefab;
    public GameObject dropDownPrefab;
    public GameObject textPrefab;
    private ConnectionsTracker tracker;
    private Camera camera;

    private float canvasHalfY = 250;
    private float canvasHalfX = 500;

    private int buttonPixelsX = 200;
    private int buttonPixelsY = 30;

    private float globalScale = 0.2f;

    private float YOffset = 5f;
    private float XOffset = 5f;

    private GameObject canvasGO;
    private Canvas canvas;

    //private List<GameObject> UIElements = new List<GameObject>();

    private List<GameObject> loadButtons = new List<GameObject>();
    private List<GameObject> saveButtons = new List<GameObject>();
    private List<GameObject> actionMapButtons = new List<GameObject>();
    private List<GameObject> variableMapping = new List<GameObject>();
    private List<GameObject> typesSelection  = new List<GameObject>();
    
    private List<ActionButtonMap> actionButtonData = new List<ActionButtonMap>();

    public GameObject GetGameObject()
    {
        return this.canvasGO;
    }

    public void Setup(Camera camera, ConnectionsTracker tracker, float globalScale = 0.2f)
    {
        this.tracker = tracker;
        this.camera = camera;
        this.globalScale = globalScale;

        this.Init();
    }

    public void Init()
    {
        if (this.buttonPrefab == null)
        {
            this.buttonPrefab = Resources.Load("Prefabs/NonCanvaces/TMP/TMPButton", typeof(GameObject)) as GameObject;
            this.inputFieldPrefab = Resources.Load("Prefabs/NonCanvaces/TMP/TMPInputField", typeof(GameObject)) as GameObject;
            this.dropDownPrefab = Resources.Load("Prefabs/NonCanvaces/TMP/TMPDropdown", typeof(GameObject)) as GameObject;
            this.textPrefab = Resources.Load("Prefabs/NonCanvaces/TMP/TMPText", typeof(GameObject)) as GameObject;
        }

        this.canvasGO = new GameObject("MainCanvas");
        this.canvas = canvasGO.AddComponent<Canvas>();
        this.canvas.worldCamera = this.camera;
        var rectT = canvas.GetComponent<RectTransform>();
        this.canvasGO.AddComponent<CanvasScaler>();
        this.canvasGO.AddComponent<GraphicRaycaster>();
        rectT.sizeDelta = new Vector2(this.canvasHalfX * 2, this.canvasHalfY * 2);

        this.DrawLoadRow(new Vector2(0, 0), out float x1, out float y1);
        this.DrawSaveCubeMenu(new Vector2(x1, 0), out float x2);
        this.DrawActionButtonMapping(new Vector2(x1 + x2, 0), out float x3);
        this.DrawVariableManagement(new Vector2(x1 + x2 + x3, 0), out float x4);
        this.DrawTypesSelection(new Vector2(x1 + x2 + x3 + x4, 0), out float x5);

        var UIElements = this.loadButtons.Concat(this.saveButtons).Concat(this.actionMapButtons).Concat(this.variableMapping).Concat(typesSelection);

        /// assuming  00 is TopRight so far, moving all elements to align
        foreach (var elem in UIElements)
        {
            elem.transform.position -= new Vector3(this.canvasHalfX, -this.canvasHalfY);
        }

        canvasGO.SetScale(new Vector3(this.globalScale, this.globalScale, this.globalScale));

        this.SetCanvasPosition(new Vector3(20, 40, 20));
    }

    #endregion 

    public void SetCanvasPosition(Vector3 pos)
    {
        this.canvasGO.transform.position = pos;
    }

    #region HIGER_LEVEL UL MAKERS

    private void DrawActionButtonMapping(Vector2 TR, out float x)
    {
        for (int i = 0; i < this.actionMapButtons.Count; i++)
        {
            GameObject.Destroy(this.actionMapButtons[i]);
        }
        this.actionMapButtons = new List<GameObject>();
        this.actionButtonData = new List<ActionButtonMap>();

        var mappings = ActionKeyPersistance.GetKeyCubeMapping();

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                GameObject mapButton = DrawButton("", new Vector2(TR.x + this.XOffset + xx * (this.XOffset + this.buttonPixelsX), TR.y - (yy * (this.YOffset + this.buttonPixelsY))));
                this.actionMapButtons.Add(mapButton);
                int keyId = yy * 3 + xx + 1;
                var node = new ActionButtonMap(keyId, mapButton, this.actionButtonData);
                var mapping = mappings.FirstOrDefault(y => y.KeyId == keyId);
                if (mapping != null)
                {
                    node.SetName(mapping.CubeName);
                }
                this.actionButtonData.Add(node);
                mapButton.GetComponent<Button>().onClick.AddListener(() => node.Select());
            }
        }

        x = (this.XOffset + this.buttonPixelsX) * 3;
    }

    private void DrawLoadRow(Vector2 TR, out float x, out float y)
    {
        for (int i = 0; i < this.loadButtons.Count; i++)
        {
            GameObject.Destroy(this.loadButtons[i]);
        }

        this.loadButtons = new List<GameObject>();

        string[] textNames = CubePersistance.GetAllSavedCubes().Select(z => z.Name).ToArray();

        for (int i = 0; i < textNames.Length; i++)
        {
            string name = textNames[i];

            GameObject main = this.DrawButton(name, new Vector2(TR.x, TR.y - i * (this.buttonPixelsY + this.YOffset)));
            GameObject delete = this.DrawButton("X", new Vector2(TR.x + this.buttonPixelsX + this.XOffset, TR.y - i * (this.buttonPixelsY + this.YOffset)), new Vector2(30, 30), Color.red);

            main.GetComponent<Button>().onClick.AddListener(() => this.OnClickLoadCube(name));
            delete.GetComponent<Button>().onClick.AddListener(() => this.OnClickDeleteCube(name));

            loadButtons.Add(main);
            loadButtons.Add(delete);
        }

        x = TR.x + this.buttonPixelsX + this.XOffset * 2 + 30;
        y = 42;
    }

    private void DrawSaveCubeMenu(Vector2 TR, out float x)
    {
        for (int i = 0; i < this.saveButtons.Count; i++)
        {
            GameObject.Destroy(this.saveButtons[i]);
        }

        this.saveButtons = new List<GameObject>();

        GameObject text = this.DrawText(new Vector2(TR.x, TR.y), "Name The Save", 20);
        GameObject input = this.DrawInputMenu(new Vector2(TR.x, TR.y - this.YOffset - this.buttonPixelsY));
        GameObject saveButton = DrawButton("Save", new Vector2(TR.x, TR.y - (this.YOffset + this.buttonPixelsY) * 2));
        saveButton.GetComponent<Button>().onClick.AddListener(() => this.OnClickSave(input.GetComponent<TMP_InputField>()));
        this.saveButtons.Add(text);
        this.saveButtons.Add(input);
        this.saveButtons.Add(saveButton);

        x = this.buttonPixelsX + this.YOffset;
    }

    private void DrawVariableManagement(Vector2 TR, out float x)
    {
        for (int i = 0; i < this.variableMapping.Count; i++)
        {
            GameObject.Destroy(this.variableMapping[i]);
        }
        
        this.variableMapping = new List<GameObject>();

        var variableNames = new string[] { ResultCanvas.PlayerMarkerVarName, ResultCanvas.PlayerForwardVarName, ResultCanvas.DroneMarkerVarName, ResultCanvas.PlayerPositionVarName};
        
        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                int index = yy * 3 + xx;

                if (index >= variableNames.Length)
                {
                    goto label;
                }
                
                GameObject variableButton = DrawButton(variableNames[index], new Vector2(TR.x + this.XOffset + xx * (this.XOffset + this.buttonPixelsX), TR.y - (yy * (this.YOffset + this.buttonPixelsY))));
                this.variableMapping.Add(variableButton);
                
                variableButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.AddVariable(variableNames[index]);
                });
            }
        }
        
        label:
        
        x = (this.XOffset + this.buttonPixelsX) * 3;
    }
    
    private void DrawTypesSelection(Vector2 TR, out float x)
    {
        for (int i = 0; i < this.typesSelection.Count; i++)
        {
            GameObject.Destroy(this.typesSelection[i]);
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
                
                GameObject variableButton = DrawButton(typeNames[index], new Vector2(TR.x + this.XOffset + xx * (this.XOffset + this.buttonPixelsX), TR.y - (yy * (this.YOffset + this.buttonPixelsY))));
                this.typesSelection.Add(variableButton);
                
                variableButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.RegisterNode<int>(DynamicSetup.Types[index]);
                });
            }
        }
        
        label:
        
        x = (this.XOffset + this.buttonPixelsX) * 3;
    }

    #endregion

    #region PRIMITIVES

    private GameObject DrawButton(string text, Vector2 pos, Vector2? sizeDelta = null, Color? color = null)
    {
        sizeDelta = sizeDelta == null ? new Vector2(this.buttonPixelsX, this.buttonPixelsY) : sizeDelta;

        GameObject button = Instantiate(this.buttonPrefab, canvasGO.transform);
        RectTransform rt = button.GetComponent<RectTransform>();
        rt.sizeDelta = sizeDelta.Value;
        button.transform.position = new Vector3(pos.x + sizeDelta.Value.x / 2, pos.y - sizeDelta.Value.y / 2, 0);
        button.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        if (color != null)
        {
            button.GetComponent<Image>().color = color.Value;
        }

        //this.UIElements.Add(button);

        return button;
    }

    private GameObject DrawText(Vector2 pos, string textInpt, int fontSize, int XX = 200, int YY = 30)
    {
        GameObject text = Instantiate(this.textPrefab, canvasGO.transform);
        RectTransform rt = text.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(this.buttonPixelsX, this.buttonPixelsY);
        text.transform.position = new Vector3(pos.x + this.buttonPixelsX / 2, pos.y - this.buttonPixelsY / 2, 0);
        TMP_Text t = text.GetComponent<TMP_Text>();
        t.text = textInpt;
        t.fontSize = fontSize;
        t.alignment = TextAlignmentOptions.Center;

        //this.UIElements.Add(text);

        return text;
    }

    private GameObject DrawInputMenu(Vector2 pos, int XX = 200, int YY = 30)
    {
        GameObject input = Instantiate(this.inputFieldPrefab, canvasGO.transform);
        RectTransform rt = input.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(XX, YY);
        input.transform.position = new Vector3(pos.x + XX / 2, pos.y - YY / 2, 0);

        //this.UIElements.Add(input);

        return input;
    }

    #endregion

    #region ON_CLICK_EVENTS 

    public void OnClickSave(TMP_InputField nameInput)
    {
        string name = nameInput.text;

        string[] existingNames = CubePersistance.GetAllSavedCubes().Select(z => z.Name).ToArray();

        if (string.IsNullOrWhiteSpace(name) || name.Length < 5 || existingNames.Contains(name))
        {
            Debug.Log("Invalid Name!");
            return;
        }

        Debug.Log("VALID Name!");

        this.tracker.Persist(name);

        this.Init();
    }

    public void OnClickDeleteCube(string name)
    {
        CubePersistance.DeleteCube(name);
        this.Init();
    }

    private void OnClickLoadCube(string cubeName)
    {
        var mappings = ActionKeyPersistance.GetKeyCubeMapping();

        if (mappings.Any(x => x.CubeName == cubeName))
        {
            Debug.Log("There is already key with that qubename!!!");
            return;
        }

        var selected = this.actionButtonData.SingleOrDefault(x => x.Selected == true);

        if (selected != null)
        {
            selected.SetName(cubeName);
            selected.Deselect();
            ActionKeyPersistance.Persist(new ActionKeyPersistance.ActionKeyPersistanceData
            {
                CubeName = cubeName,
                KeyId = selected.ID, 
            });
        }
    }

    #endregion

    #region DATA_CLASSES

    public class ActionButtonMap
    {
        public ActionButtonMap(int id, GameObject gameObject, List<ActionButtonMap> all)
        {
            this.ID = id;
            this.GameObject = gameObject;
            this.all = all;
        }

        private List<ActionButtonMap> all;

        public int ID { get; set; }

        public GameObject GameObject { get; set; }

        public string Name { get; set; } = null;

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

