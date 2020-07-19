using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputCanvas
{
    private Camera camera;
    private GameObject constantAndVariablePanelPrefab;
    private float constantsScale = 0.3f;
    private List<InputElements> inputs = new List<InputElements>();
    private bool inputsShowing = false;
    private Transform localParent;

    public InputCanvas(Camera camera, GameObject constantAndVariablePanelPrefab, Transform parent)
    {
        this.camera = camera;
        this.constantAndVariablePanelPrefab = constantAndVariablePanelPrefab;
        this.localParent = new GameObject("Contants Parent!").transform;
        this.localParent.SetParent(parent); 
    }

    public InputElements CreateInputCanvas(object value, int ID, WorldSpaceUI worldSpaceUI, bool isVariable, string name = "constant")
    {
        GameObject obj = GameObject.Instantiate(this.constantAndVariablePanelPrefab);
        Canvas can = obj.GetComponent<Canvas>();
        can.worldCamera = this.camera;
        GameObject buttonGo = obj.transform.Find("Button").gameObject;
        Button button = buttonGo.GetComponent<Button>();
        GameObject textGO = buttonGo.transform.Find("Text").gameObject;
        TMP_Text text = textGO.GetComponent<TMP_Text>();
        if (isVariable)
        {
            text.text = name;
        }
        else
        {
            text.text = value.ToString();
        }

        DirectInputNode nodeBe = buttonGo.AddComponent<DirectInputNode>();
        RectTransform rt = obj.GetComponent<RectTransform>();
        InputElements result = new InputElements(obj, text, nodeBe, rt, button);
        obj.transform.SetParent(this.localParent);
        ///element scaling!
        rt.localScale *= this.constantsScale;
        nodeBe.Setup(value, ID, worldSpaceUI, result, isVariable, name);
        this.inputs.Add(result);
        return result;
    }

    public async Task InputsDisplay(Vector3 pos, ParameterNode node)
    {
        int columnCount = 2;
        float buttonX = 1.5f;
        float buttonY = 1f;
        /// Scaling
        buttonX *= this.constantsScale;
        buttonY *= this.constantsScale;
        ///...
        int rows = (int)Mathf.Ceil((float)this.inputs.Count / columnCount);
        float wholeY = rows * buttonY;
        float wholeX = buttonX * columnCount;


        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columnCount; x++)
            {
                int index = y * columnCount + x;

                if (index >= this.inputs.Count)
                {
                    break;
                }

                InputElements c = this.inputs[index];

                c.ParentObject.SetActive(false);

                float yy = y * buttonY - wholeY / 2 + buttonY / 2;
                float xx = x * buttonX - wholeX / 2 + buttonX / 2;

                c.RectTransform.localPosition = new Vector3(xx, yy, 0);

                c.Node.FixColorBeforeShow(node);
            }
        }

        this.localParent.LookAt(this.localParent.transform.position + this.camera.transform.forward);

        Vector3 offset = (this.camera.transform.position - pos).normalized;

        this.localParent.position = pos + offset;

        this.inputsShowing = true;

        /// Enabaling the constant canvases after some time so one does not get automatically clicked
        await Task.Delay(100);

        foreach (var item in this.inputs)
        {
            item.ParentObject.SetActive(true);
        }

        //Debug.Log("Display");
    }

    public void InputsHide()
    {
        for (int i = 0; i < this.inputs.Count; i++)
        {
            InputElements c = this.inputs[i];
            c.ParentObject.SetActive(false);
        }

        this.inputsShowing = false;

        //Debug.Log("Hide");
    }

    #region DATA_CLASSES

    public InputElements[] GetInputs()
    {
        return this.inputs.ToArray();
    }

    public bool AreInputsShowing()
    {
        return this.inputsShowing;
    }

    public class InputElements
    {
        public GameObject ParentObject { get; set; }

        public TMP_Text text { get; set; }

        public DirectInputNode Node { get; set; }

        public RectTransform RectTransform { get; set; }

        public Button Button { get; set; }

        public bool Used { get; set; } = false;

        public InputElements(GameObject canvas, TMP_Text text, DirectInputNode node, RectTransform rectTransform, Button button)
        {
            this.RectTransform = rectTransform;
            this.ParentObject = canvas;
            this.text = text;
            this.Node = node;
            this.Button = button;
        }
    }

    #endregion
}

