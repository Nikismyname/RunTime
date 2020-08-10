// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class SpellcraftProcUIRPG : MonoBehaviour
// {
//     #region INIT
//
//     public GameObject buttonPrefab;
//     public GameObject inputFieldPrefab;
//     public GameObject dropDownPrefab;
//     public GameObject textPrefab;
//     private ConnectionsTracker tracker; 
//     private Camera camera;
//
//     private float canvasHalfY = 250;
//     private float canvasHalfX = 500;
//
//     private int buttonPixelsX = 200;
//     private int buttonPixelsY = 30;
//
//     private float globalScale = 0.2f;
//
//     private float YOffset = 5f;
//     private float XOffset = 5f;
//
//     private GameObject canvasGO;
//     private Canvas canvas;
//
//     private List<GameObject> UIElements = new List<GameObject>();
//
//     private List<GameObject> loadButtons = new List<GameObject>();
//     private List<GameObject> saveButtons = new List<GameObject>();
//
//     public GameObject GetGameObject()
//     {
//         return this.canvasGO;
//     }
//
//     public void Setup(Camera camera, ConnectionsTracker tracker , float globalScale = 0.2f)
//     {
//         this.tracker = tracker;
//         this.camera = camera;
//         this.globalScale = globalScale;
//
//         this.Init();
//     }
//
//     public void Init()
//     {
//         this.canvasGO = new GameObject("MainCanvas");
//         this.canvas = canvasGO.AddComponent<Canvas>();
//         this.canvas.worldCamera = this.camera;
//         var rectT = canvas.GetComponent<RectTransform>();
//         this.canvasGO.AddComponent<CanvasScaler>();
//         this.canvasGO.AddComponent<GraphicRaycaster>();
//         rectT.sizeDelta = new Vector2(this.canvasHalfX * 2, this.canvasHalfY * 2);
//
//         this.CreateLoadRow(new Vector2(0, 0), out float x1, out float y1);
//         this.DrawSaveCubeMenu(new Vector2(x1, 0));
//
//         /// assuming  00 is TopRight so far, moving all elements to align
//         foreach (var elem in this.UIElements)
//         {
//             elem.transform.position -= new Vector3(this.canvasHalfX, -this.canvasHalfY);
//         }
//
//         canvasGO.SetScale(new Vector3(this.globalScale, this.globalScale, this.globalScale));
//
//         this.SetCanvasPosition(new Vector3(20, 40, 20));
//     }
//     
//     #endregion
//
//     private void CreateLoadRow(Vector2 TR, out float x, out float y)
//     {
//         for (int i = 0; i < this.loadButtons.Count; i++)
//         {
//             GameObject.Destroy(this.loadButtons[i]);
//         }
//
//         this.loadButtons = new List<GameObject>();
//
//         string[] textNames = CubePersistance.GetAllSavedCubes().Select(z=> z.Name).ToArray();
//
//         for (int i = 0; i < textNames.Length; i++)
//         {
//             string name = textNames[i];
//
//             GameObject main = this.DrawButton(name, new Vector2(TR.x, TR.y - i * (this.buttonPixelsY + this.YOffset)));
//             GameObject delete = this.DrawButton("X", new Vector2(TR.x + this.buttonPixelsX + this.XOffset, TR.y - i * (this.buttonPixelsY + this.YOffset)), new Vector2(30, 30), Color.red);
//
//             loadButtons.Add(main);
//             loadButtons.Add(delete);
//         }
//
//         x = TR.x + this.buttonPixelsX + this.XOffset * 2 + 30;
//         y = 42;
//     }
//
//     private void DrawSaveCubeMenu(Vector2 TR)
//     {
//         for (int i = 0; i < this.saveButtons.Count; i++)
//         {
//             GameObject.Destroy(this.saveButtons[i]);
//         }
//
//         this.saveButtons = new List<GameObject>();
//
//         GameObject text = this.DrawText(new Vector2(TR.x, TR.y), "Name The Save", 20);
//         GameObject input = this.DrawInputMenu(new Vector2(TR.x, TR.y - this.YOffset - this.buttonPixelsY));
//         GameObject saveButton = DrawButton("Save", new Vector2(TR.x, TR.y - (this.YOffset + this.buttonPixelsY) * 2));
//         saveButton.GetComponent<Button>().onClick.AddListener(()=>this.OnClickSave(input.GetComponent<TMP_InputField>()));
//         this.saveButtons.Add(text);
//         this.saveButtons.Add(input);
//         this.saveButtons.Add(saveButton);
//         
//     }
//
//     #region PRIMITIVES
//
//     private GameObject DrawButton(string text, Vector2 pos, Vector2? sizeDelta = null, Color? color = null)
//     {
//         sizeDelta = sizeDelta == null ? new Vector2(this.buttonPixelsX, this.buttonPixelsY) : sizeDelta;
//
//         GameObject button = Instantiate(this.buttonPrefab, canvasGO.transform);
//         RectTransform rt = button.GetComponent<RectTransform>();
//         rt.sizeDelta = sizeDelta.Value;
//         button.transform.position = new Vector3(pos.x + sizeDelta.Value.x / 2, pos.y - sizeDelta.Value.y / 2, 0);
//         button.transform.Find("Text").GetComponent<TMP_Text>().text = text;
//         if (color != null)
//         {
//             button.GetComponent<Image>().color = color.Value;
//         }
//
//         this.UIElements.Add(button);
//
//         return button; 
//     }
//
//     private GameObject DrawText(Vector2 pos, string textInpt, int fontSize, int XX = 200, int YY = 30)
//     {
//         GameObject text = Instantiate(this.textPrefab, canvasGO.transform);
//         RectTransform rt = text.GetComponent<RectTransform>();
//         rt.sizeDelta = new Vector2(this.buttonPixelsX, this.buttonPixelsY);
//         text.transform.position = new Vector3(pos.x + this.buttonPixelsX / 2, pos.y - this.buttonPixelsY / 2, 0);
//         TMP_Text t = text.GetComponent<TMP_Text>();
//         t.text = textInpt;
//         t.fontSize = fontSize;
//         t.alignment = TextAlignmentOptions.Center;
//
//         this.UIElements.Add(text);
//
//         return text;
//     }
//
//     private GameObject DrawInputMenu(Vector2 pos, int XX = 200, int YY = 30)
//     {
//         GameObject input = Instantiate(this.inputFieldPrefab, canvasGO.transform);
//         RectTransform rt = input.GetComponent<RectTransform>();
//         rt.sizeDelta = new Vector2(XX, YY);
//         input.transform.position = new Vector3(pos.x + XX / 2, pos.y - YY / 2, 0);
//
//         this.UIElements.Add(input);
//
//         return input;
//     }
//
//     #endregion
//
//     #region BUTTONS 
//
//     public void OnClickSave(TMP_InputField nameInput)
//     {
//         string name = nameInput.text;
//
//         string[] existingNames = CubePersistance.GetAllSavedCubes().Select(z => z.Name).ToArray();
//
//         if (string.IsNullOrWhiteSpace(name) || name.Length < 5 || existingNames.Contains(name))
//         {
//             Debug.Log("Invalid Name!");
//             return;
//         }
//
//         Debug.Log("VALID Name!");
//
//         this.tracker.Persist(name);
//
//         this.Init();
//     }
//
//     #endregion
//
//     #region PUBLIC INTERFACE
//     
//     public void SetCanvasPosition(Vector3 pos)
//     {
//         this.canvasGO.transform.position = pos;
//     }
//     
//     #endregion
// }
//
