#region INIT
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ManageActionsButtons : MonoBehaviour
{
    public GameObject buttonPreFab;
    public GameObject labelPreFab;
    public GameObject inputPrefab;
    private GameObject colorPicker;
    private Transform parentTransform;
    private GameObject currentTarget;
    private GameObject previousTarget;

    private float marginX;
    private float marginY;
    private float marginLableToItsButtons;
    private float marginBetweenButtonRows;
    private float horizontalLength;
    private float buttonX;
    private float buttonY;
    private float labelY;
    private float labelX;
    private float inputX;
    private float inputY;

    private Dictionary<GameObject, List<UiMonoGroupInformation>> monosPerObjectData = new Dictionary<GameObject, List<UiMonoGroupInformation>>();
    private List<ColorSelectionButton> collorPickerButtons = new List<ColorSelectionButton>();

    private RectTransform parentRT;
    private Main ms;

    private float startY;

    void Start()
    {
        var globalParent = new GameObject("TestParent");
        globalParent.transform.SetParent(gameObject.transform, false);
        globalParent.transform.position = gameObject.transform.position;
        this.parentTransform = globalParent.transform;

        this.marginX = 10;
        this.marginY = 20;
        this.marginLableToItsButtons = 0;
        this.marginBetweenButtonRows = 0;
        this.horizontalLength = gameObject.GetComponent<RectTransform>().sizeDelta.x;
        this.buttonX = (horizontalLength - 3 * this.marginX) / 2;
        this.buttonY = this.buttonX / 5;
        this.labelY = this.buttonY;
        this.labelX = this.buttonX * 2 + marginX;
        this.inputX = buttonX;
        this.inputY = this.buttonY;

        this.startY = -this.marginY;

        var mainGO = GameObject.Find("Main");
        this.ms = mainGO.GetComponent<Main>();
        this.colorPicker = mainGO.GetComponent<ReferenceBuffer>().ColorPicker;

        this.parentRT = gameObject.GetComponent<RectTransform>();

        this.currentTarget = null;
        this.previousTarget = null;

    }
    #endregion

    public void SetTarget(GameObject newTarget)
    {
        //Debug.Log("SetTarget: " + newTarget.name);
        if (newTarget == this.currentTarget)
        {
            return;
        }

        this.previousTarget = this.currentTarget;
        this.currentTarget = newTarget;

        this.colorPicker.SetActive(false);

        //setting the data structire for this targets mono so we do not have to check later
        if (!this.monosPerObjectData.ContainsKey(currentTarget))
        {
            this.monosPerObjectData[currentTarget] = new List<UiMonoGroupInformation>();
        }

        this.ClearUI(this.previousTarget);
        this.DisplayInterfaceForTarger(this.currentTarget);
    }

    #region REGISTER_MONOS
    public void RegisterNewOrChangedMono(UiMonoWithMethods monoData)
    {
        var target = monoData.Object;

        if (!this.monosPerObjectData.ContainsKey(target))
        {
            this.monosPerObjectData[target] = new List<UiMonoGroupInformation>();
        }

        var list = this.monosPerObjectData[target];
        if (list.Any(x => x.MonoName == monoData.MonoName))
        {
            this.RemoveDestroyedMono(target, monoData.MonoName, false);
        }

        var createResult = this.CreateScriptUI(monoData.MonoName, monoData.Methods);

        var monoGroup = new UiMonoGroupInformation
        {
            MonoName = monoData.MonoName,
            WholeHeight = createResult.FinalHeight,
            Methods = createResult.Parent,

            MonoButtonLabel = createResult.ButtonLabel,
            Collapsed = false,
            CollapsedHeight = createResult.CollapsedHeight,

            GrandParent = createResult.GrandParent,
        };

        createResult.LabelButtonScritp.SetUp(monoGroup, this);

        list.Add(monoGroup);

        if (this.currentTarget!= null && this.currentTarget == target) {
            this.DisplayInterfaceForTarger(target);
        }
    }

    //Removes it from the dict as well as destroys the object visualisation
    public void RemoveDestroyedMono(GameObject target, string monoName, bool reorder = true)
    {
        if (!this.monosPerObjectData.ContainsKey(target))
        {
            Debug.Log("Can not remove mono, there is no such target overhere!");
            return;
        }

        var list = this.monosPerObjectData[target];
        if (!list.Any(x => x.MonoName == monoName))
        {
            Debug.Log("Can not remove mono, there is no such mono on the giver target overhere!");
            return;
        }

        var mono = list.Single(x => x.MonoName == monoName);

        Destroy(mono.GrandParent);
        list.Remove(mono);

        if (reorder && this.currentTarget == target)
        {
            this.DisplayInterfaceForTarger(target);
        }
    }

    private void ClearUI(GameObject objToClear)
    {
        if (objToClear == null)
        {
            return;
        }

        var listToClear = this.monosPerObjectData[objToClear];

        foreach (var item in listToClear)
        {
            item.Methods.SetActive(false);
            item.MonoButtonLabel.SetActive(false);
        }
    }

    //Check again when this gets called it seems like way too often.
    public void DisplayInterfaceForTarger(GameObject displayTarget, bool useCurrentTarget = false)
    {
        //Debug.Log("Display Interface: " + displayTarget.name);
        if (useCurrentTarget)
        {
            displayTarget = this.currentTarget;
        }

        /// aranging the current interface
        var toReorder = this.monosPerObjectData[displayTarget];
        var initialY = this.marginY;
        for (int i = 0; i < toReorder.Count; i++)
        {
            var item = toReorder[i];

            item.GrandParent.transform.position =
                item.GrandParent.transform.parent.position - new Vector3(0, initialY, 0);

            var height = 0f;
            if (item.Collapsed)
            {
                height = item.CollapsedHeight;
            }
            else
            {
                height = item.WholeHeight;
            }

            initialY += height;
        }

        /// showing the current interface
        foreach (var item in toReorder)
        {
            if (item.Collapsed)
            {
                item.Methods.SetActive(false);
                item.MonoButtonLabel.SetActive(true);
            }
            else
            {
                item.Methods.SetActive(true);
                item.MonoButtonLabel.SetActive(true);
            }
        }

        var totalHeight = initialY;

        /// expanding the box if need be
        var y = parentRT.sizeDelta.y;
        var x = parentRT.sizeDelta.x;

        if (y < totalHeight)
        {
            parentRT.sizeDelta = new Vector2(x, totalHeight);
        }
    }
    #endregion

    #region CREATE_SCRIPT_UI
    private CreateMonUIResult CreateScriptUI(string monoName, UiMethodNameWithParameters[] methods)
    {
        var localGrandParent = new GameObject(monoName + "GrandParent");
        localGrandParent.transform.SetParent(this.parentTransform, false);
        localGrandParent.transform.position = this.parentTransform.position;

        var localParent = new GameObject(monoName);
        localParent.transform.SetParent(localGrandParent.transform, false);
        localParent.transform.position = localGrandParent.transform.position;
        var localTransform = localParent.transform;

        var localHeight = 0f;

        ///assigning the label to the grandparent not the parent
        var labelInfo = this.SpawnOneLable(new Vector2(this.marginX, localHeight), monoName, localGrandParent.transform, true, out GameObject intButtonLabel, true);
        var lableButtonScript = labelInfo.labelButtonScript;

        localHeight -= labelInfo.Height;

        ///Collectiong All The method button behaviurs so that we can extract all colorButton scripts 
        ///from them
        List<MethodButtonBehaviour> methodButtonBehs = new List<MethodButtonBehaviour>();

        ///all others are assigned to the parent so we can deactivate them seperately
        for (int i = 0; i < methods.Length; i += 2)
        {
            if (i + 1 == methods.Length)
            {
                var method = methods[i];
                var pos = new Vector2(marginX, localHeight);
                var beh = this.SpawnOneButton(pos, method.Name, monoName, localTransform);
                pos.y -= this.buttonY;
                var height = this.SpawnAllInputs(pos, beh, method, localTransform);
                localHeight -= (height + buttonY + marginY);

                methodButtonBehs.Add(beh);
            }
            else
            {
                var method1 = methods[i];
                var method2 = methods[i + 1];

                var pos1 = new Vector2(marginX, localHeight);
                var beh1 = this.SpawnOneButton(pos1, method1.Name, monoName, localTransform);
                pos1.y -= this.buttonY;
                var height1 = this.SpawnAllInputs(pos1, beh1, method1, localTransform);

                var pos2 = new Vector2(marginX * 2 + this.buttonX, localHeight);
                var beh2 = this.SpawnOneButton(pos2, method2.Name, monoName, localTransform);
                pos2.y -= this.buttonY;
                var height2 = this.SpawnAllInputs(pos2, beh2, method2, localTransform);
                localHeight -= (Mathf.Max(height1, height2) + buttonY + marginY);

                methodButtonBehs.Add(beh1);
                methodButtonBehs.Add(beh2);
            }
        }

        ///Collecting all the color scripts so we can close Color Picker on colapse
        foreach (var methodScript in methodButtonBehs)
        {
            foreach (var colorButtonScript in methodScript.ColorParamaters)
            {
                lableButtonScript.ColorSelectionButtons.Add(colorButtonScript.colorScript);
            }
        }

        /// newly created item whould be activated later;
        localParent.SetActive(false);
        intButtonLabel.SetActive(false);

        var result = new CreateMonUIResult
        {
            ButtonLabel = intButtonLabel,
            CollapsedHeight = labelInfo.Height,
            FinalHeight = -localHeight,
            GrandParent = localGrandParent,
            LabelButtonScritp = labelInfo.labelButtonScript,
            Parent = localParent,
        };

        return result;
    }
    #endregion

    #region SPAWN
    private MethodButtonBehaviour SpawnOneButton(Vector2 pos, string method, string mono, Transform parent)
    {
        var button = Instantiate(this.buttonPreFab, parent);

        button.GetComponent<RectTransform>().sizeDelta = new Vector2(this.buttonX, this.buttonY);

        var position = new Vector2(pos.x + this.buttonX / 2, pos.y - this.buttonY / 2);
        button.GetComponent<RectTransform>().anchoredPosition = position;
        button.GetComponentInChildren<Text>().text = method;

        button.GetComponent<Image>().color = new Color32(229, 200, 25, 255);
        //button.GetComponent<Button>().image.color = new Color(229, 200, 25);
        //var thing = button.GetComponent<Button>().colors;
        //thing.normalColor = new Color(229, 200, 25);
        //button.GetComponent<Button>().colors = thing;

        var script = button.AddComponent<MethodButtonBehaviour>();
        script.SetUp(mono, method);

        return script;
    }

    private ActionUiLabelButtonWithHeight SpawnOneLable(
        Vector2 pos,
        string text,
        Transform parent,
        bool button,
        out GameObject buttonLabel,
        bool monoName)
    {
        var result = new ActionUiLabelButtonWithHeight();

        var label = Instantiate(this.labelPreFab, parent);
        var labelRT = label.GetComponent<RectTransform>();
        var labelText = label.GetComponent<Text>();
        labelText.text = text;

        if (monoName)
        {
            labelRT.sizeDelta = new Vector2(this.labelX, this.labelY);
            labelText.fontSize = 20;
            labelText.alignment = TextAnchor.MiddleCenter;
        }
        else
        {
            labelRT.sizeDelta = new Vector2(this.buttonX, this.labelY);
        }

        var localX = labelRT.sizeDelta.x;
        var localY = labelRT.sizeDelta.y;

        var position = new Vector2(pos.x + localX / 2, pos.y - localY / 2);
        label.GetComponent<RectTransform>().anchoredPosition = position;


        if (button)
        {
            label.AddComponent<Button>();
            var script = label.AddComponent<LabelButtonMonoName>();
            result.labelButtonScript = script;
            buttonLabel = label;
        }
        else
        {
            result.labelButtonScript = null;
            buttonLabel = null;
        }

        result.Height = this.labelY + this.marginLableToItsButtons;

        return result;
    }

    private InputField SpawnOneInput(Vector2 pos, string parameterName, Transform parent, string placeHolderText = null)
    {
        var input = Instantiate(this.inputPrefab, parent);
        input.GetComponent<RectTransform>().sizeDelta = new Vector2(this.inputX, this.inputY);
        var position = new Vector2(pos.x + this.inputX / 2, pos.y - this.inputY / 2);
        input.GetComponent<RectTransform>().anchoredPosition = position;
        var inputField = input.GetComponent<InputField>();
        if (placeHolderText != null)
        {
            var phText = input.transform.Find("Placeholder").GetComponent<Text>();
            phText.text = placeHolderText;
        }
        return inputField;
    }

    private ColorSelectionButton SpawnOneColorButton(Vector2 pos, Transform parent)
    {
        var button = Instantiate(this.buttonPreFab, parent);
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(this.buttonX, this.buttonY);
        var position = new Vector2(pos.x + this.buttonX / 2, pos.y - this.buttonY / 2);
        button.GetComponent<RectTransform>().anchoredPosition = position;
        button.GetComponentInChildren<Text>().text = "";
        button.GetComponent<Image>().color = Color.white;
        var script = button.AddComponent<ColorSelectionButton>();
        this.collorPickerButtons.Add(script);
        script.SetUp(this.collorPickerButtons, this.collorPickerButtons.Count);
        return script; // returning so it can be registered in the Method Button Script
    }

    private float SpawnAllInputs(Vector2 pos, MethodButtonBehaviour parentBehaviour, UiMethodNameWithParameters method, Transform parent)
    {
        var overallHeight = 0f;

        foreach (var parameter in method.Parameters)
        {
            var height = this.SpawnInputsSingleParameter(pos, parameter, parentBehaviour, parent);
            overallHeight += height;
            pos.y -= height;
        }

        return overallHeight;
    }

    private float SpawnInputsSingleParameter(Vector2 pos, UiParameterWithType parameter, MethodButtonBehaviour parentBehaviour, Transform parent)
    {
        float overallHeight = 0;
        var height = this.SpawnOneLable(
            pos,
            parameter.Name + " " + this.ShortenPropTypes(parameter.Type.Name),
            parent,
            false,
            out GameObject _notused,
            false).Height;

        pos.y -= height; overallHeight += height;

        if (parameter.Type == typeof(Vector3))
        {
            var inputField1 = this.SpawnOneInput(pos, parameter.Name, parent, "x: float");
            pos.y -= this.inputY;
            var inputField2 = this.SpawnOneInput(pos, parameter.Name, parent, "y: float");
            pos.y -= this.inputY;
            var inputField3 = this.SpawnOneInput(pos, parameter.Name, parent, "z: float");

            parentBehaviour.RegisterNonColorParamater(parameter.Name, inputField1);
            parentBehaviour.RegisterNonColorParamater(parameter.Name, inputField2);
            parentBehaviour.RegisterNonColorParamater(parameter.Name, inputField3);

            overallHeight += 3 * this.inputY;
        }

        if (parameter.Type == typeof(Vector2))
        {
            var inputField1 = this.SpawnOneInput(pos, parameter.Name, parent, "x: float");
            pos.y -= this.inputY;
            var inputField2 = this.SpawnOneInput(pos, parameter.Name, parent, "y: float");

            parentBehaviour.RegisterNonColorParamater(parameter.Name, inputField1);
            parentBehaviour.RegisterNonColorParamater(parameter.Name, inputField2);

            overallHeight += 2 * this.inputY;
        }

        if (
            parameter.Type == typeof(string) ||
            parameter.Type == typeof(float) ||
            parameter.Type == typeof(decimal) ||
            parameter.Type == typeof(int) ||
            parameter.Type == typeof(short) ||
            parameter.Type == typeof(long) ||
            parameter.Type == typeof(char)
        )
        {
            var inputField = this.SpawnOneInput(pos, parameter.Name, parent, parameter.Type.Name);
            parentBehaviour.RegisterNonColorParamater(parameter.Name, inputField);
            overallHeight += this.inputY;
        }

        if (parameter.Type == typeof(Color))
        {
            var colorSelectionScript = this.SpawnOneColorButton(pos, parent);
            parentBehaviour.RegisterColorParam(parameter.Name, colorSelectionScript);
            overallHeight += this.inputY;
        }

        return overallHeight;
    }
    #endregion

    #region HELPERS
    public string ShortenPropTypes(string propType)
    {
        const int maxLenght = 10;

        if (propType.Length <= maxLenght)
        {
            return propType;
        }

        var indecies = new List<int>();
        for (int i = 0; i < propType.Length; i++)
        {
            var letter = propType[i];

            if (char.IsUpper(letter))
            {
                indecies.Add(i);
            }
        }

        var charArr = new List<char>();
        foreach (var ind in indecies)
        {
            charArr.Add(propType[ind]);
        }

        var remainingLength = maxLenght - indecies.Count;

        var substring = propType.Substring(indecies.Last() + 1);

        for (int i = 0; i < substring.Length; i++)
        {
            var letter = substring[i];
            charArr.Add(letter);
        }

        var result = new string(charArr.ToArray());

        return result;
    }
    #endregion

    #region END_BRACKET
}
#endregion