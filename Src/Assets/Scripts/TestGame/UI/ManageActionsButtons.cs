#region INIT
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

///Attached to ScrollableActions////ActionsContent

/// <summary>
///    This script is attached to scrollable panel in the UI. 
/// It recives information from the Main <see cref="Main"> about the 
/// targets and what monos are attached to them and creates a UI that 
/// reflects that information for the currently selected target. 
/// 
///    It displays the name of every attached mono, buttons for every 
/// method of those monos as well as inputs for every parameter of 
/// every method. Clicking the method button invokes the method with 
/// the entered inputs.
/// </summary>
public class ManageActionsButtons : MonoBehaviour
{
    public GameObject buttonPreFab;
    public GameObject labelPreFab;
    public GameObject inputPrefab;
    private GameObject colorPicker;
    private GameObject currentTarget;
    private GameObject previousTarget;
    private Transform parentTransform;
    private InputFocusManager inputFocusManager;

    private float marginX;
    private float marginY;
    private float marginLableToItsButtons;
    private float xLength;
    private float buttonX;
    private float buttonY;
    private float labelY;
    private float labelX;
    private float inputX;
    private float inputY;

    /// Here we hold all the attached mono info for every target GameObject 
    private Dictionary<GameObject, List<UiMonoGroupInformation>> monosPerObjectData = new Dictionary<GameObject, List<UiMonoGroupInformation>>();
    private List<ColorSelectionButton> collorPickerButtons = new List<ColorSelectionButton>();

    private RectTransform parentRT;

    private ShowActionsBehaviour visibilityManager;

    void Start()
    {
        /// Creating the paraent object that will hold all UI Elements
        var globalParent = new GameObject("TestParent");
        globalParent.transform.SetParent(gameObject.transform, false);
        globalParent.transform.position = gameObject.transform.position;
        this.parentTransform = globalParent.transform;
        ///...

        this.marginX = 10;
        this.marginY = 20;
        this.marginLableToItsButtons = 0;
        this.xLength = gameObject.GetComponent<RectTransform>().sizeDelta.x;
        /// Calculating the dimentions for lables, buttons and inputs
        this.buttonX = (xLength - 3 * this.marginX) / 2;
        this.buttonY = this.buttonX / 5;
        this.labelY = this.buttonY;
        this.labelX = this.buttonX * 2 + marginX;
        this.inputX = buttonX;
        this.inputY = this.buttonY;
        ///...

        /// Cashing scripts
        var main = GameObject.Find("Main");
        ///InputFocusManager is used to register inputs so when a shortcut is pressed
        ///during typing, it does not trigger the shortcut
        this.inputFocusManager = main.GetComponent<InputFocusManager>();
        this.colorPicker = main.GetComponent<ReferenceBuffer>().ColorPicker;
        ///...

        /// Cashing the panel RectTransform
        this.parentRT = gameObject.GetComponent<RectTransform>();

        this.currentTarget = null;
        this.previousTarget = null;

        this.visibilityManager = GameObject.Find("ShowActionsButton").GetComponent<ShowActionsBehaviour>();
    }
    #endregion

    #region SET_TARGET
    /// <summary>
    /// This mothod is called from main <see cref="Main"> to indicate that new target 
    /// has been selected. As a result, new actions need to be displayed.
    /// </summary>
    public void SetTarget(GameObject newTarget)
    {
        /// Setting the data structire for this target's mono so we do not have to check later.
        if (!this.monosPerObjectData.ContainsKey(newTarget))
        {
            this.monosPerObjectData[newTarget] = new List<UiMonoGroupInformation>();
        }

        /// Managing the visibility: if no items to display close the thing, else open it if closed!
        if (this.monosPerObjectData[newTarget].Count > 0)
        {
            visibilityManager.Open();
        }
        else
        {
            visibilityManager.Close();
        }

        /// If a the new target for some reason is the current target here, ignore.
        if (newTarget == this.currentTarget)
        {
            return;
        }

        /// Keeping a reference to the preveous target as well as setting the new current target.
        this.previousTarget = this.currentTarget;
        this.currentTarget = newTarget;
        ///...

        /// In case a color picker is active at the time of switching, disable it. 
        this.colorPicker.SetActive(false);

        /// Hide all the UI for the previous target!
        this.ClearUI(this.previousTarget);
        /// Display the actual interface!
        this.DisplayInterfaceForTarger(this.currentTarget);
    }

    #endregion

    #region REGISTER_OR_CHANGE_MONOS
    /// <summary>
    /// Registers new monos as well as changes to previous monos and creates 
    /// the interface necessary for said mono.
    /// </summary>
    public void RegisterNewOrChangedMono(UiMonoWithMethods monoData)
    {
        var target = monoData.Object;

        /// If new target set up the data structure for it.
        if (!this.monosPerObjectData.ContainsKey(target))
        {
            this.monosPerObjectData[target] = new List<UiMonoGroupInformation>();
        }

        var listOfMonoData = this.monosPerObjectData[target];
        /// If we already have data with the same mono name, destroy it.
        if (listOfMonoData.Any(x => x.MonoName == monoData.MonoName))
        {
            this.RemoveDestroyedMono(target, monoData.MonoName, false);
        }

        /// Creates the inteface as well as meta data.
        var createResult = this.CreateScriptUI(monoData.MonoName, monoData.Methods);

        ///All the data we need to visualise the UI
        var monoGroup = new UiMonoGroupInformation
        {
            MonoName = monoData.MonoName,
            WholeHeight = createResult.FinalHeight,
            Methods = createResult.Parent,
            MonoButtonLabel = createResult.ButtonLabel,
            Collapsed = false,
            CollapsedHeight = createResult.CollapsedHeight,
            GrandParent = createResult.GrandParent,
            Source = monoData.Source,
        };

        /// Setting up the mono name label to be able to callapse and expand the method 
        createResult.LabelButtonScritp.SetUp(monoGroup, this);

        /// Storing the resulting UI data
        listOfMonoData.Add(monoGroup);

        /// If the new UI ifromation is for current target - display the updated version 
        if (this.currentTarget != null && this.currentTarget == target)
        {
            this.DisplayInterfaceForTarger(target);
        }
    }
    #endregion

    #region REMOVE_DESTROYED_MONO
    /// <summary>
    /// Removes it from the store as well as destroys the UI elements.
    /// </summary>
    public void RemoveDestroyedMono(GameObject target, string monoName, bool reorder = true)
    {
        /// If the target is not in monosPerObjectData return.
        if (!this.monosPerObjectData.ContainsKey(target))
        {
            Debug.Log("Can not remove mono, there is no such target over here!");
            return;
        }

        /// Get the list of mono for given target GameObject
        var nomosList = this.monosPerObjectData[target];

        /// If there is no mono with that name for target GameObject, return
        if (!nomosList.Any(x => x.MonoName == monoName))
        {
            Debug.Log("Can not remove mono, there is no such mono on the giver target over here!");
            return;
        }

        /// Getting the mono info that needs to be destroyed  
        var mono = nomosList.Single(x => x.MonoName == monoName);

        /// Destroying the generated UI.
        Destroy(mono.GrandParent);
        /// Removing the mono from the list.
        nomosList.Remove(mono);

        /// If we want to reorder and are currently displaying for the same targetGO, redisplay the updated UI
        if (reorder && this.currentTarget == target)
        {
            this.DisplayInterfaceForTarger(target);
        }
    }
    #endregion

    #region CLEAR_UI
    /// <summary>
    /// Hides All UI elements that for given targetGO
    /// </summary>
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
    #endregion

    #region DISPLAY_INTERFACE_FOR_TARGET
    ///TODO: Check again when this gets called it seems like way too often.
    /// <summary>
    /// Displays the already created interface for a given target.
    /// </summary>
    public void DisplayInterfaceForTarger(GameObject displayTarget, bool useCurrentTarget = false)
    {
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
            }
            else
            {
                item.Methods.SetActive(true);
            }

            item.MonoButtonLabel.SetActive(true);
        }

        var totalHeight = initialY;

        /// expanding the box if need be
        var y = parentRT.sizeDelta.y;
        var x = parentRT.sizeDelta.x;
        if (y < totalHeight)
        {
            parentRT.sizeDelta = new Vector2(x, totalHeight);
        }
        ///...
    }
    #endregion

    #region CREATE_SCRIPT_UI
    /// <summary>
    /// Creates the UI to visualise given mono's methods as buttons and their paramaters as inputs
    /// </summary>
    /// <param name="monoName">The name of the mono script</param>
    /// <param name="methods">Information abouth the methods in the script</param>
    /// <returns>The generated UI as well as meta data for it</returns>
    private CreateMonUIResult CreateScriptUI(string monoName, UiMethodNameWithParameters[] methods)
    {
        /// localGrandParent hold the whole UI
        /// Directly howds only the buttonLabel(the mono name) and the local parent
        /// the local parent holds everything else (method buttons and parameter inputs)
        /// clicking on the buttonLabel collapses the local parent
        GameObject localGrandParent = new GameObject(monoName + "GrandParent");
        localGrandParent.transform.SetParent(this.parentTransform, false);
        localGrandParent.transform.position = this.parentTransform.position;

        GameObject localParent = new GameObject(monoName);
        localParent.transform.SetParent(localGrandParent.transform, false);
        localParent.transform.position = localGrandParent.transform.position;
        Transform localTransform = localParent.transform;

        float localHeight = 0f;

        ///assigning the label to the grandparent not the parent
        ActionUiLabelButtonWithHeight labelInfo = this.SpawnOneLable(new Vector2(this.marginX, localHeight), monoName, localGrandParent.transform, true, out GameObject intButtonLabel, true);
        LabelButtonMonoName lableButtonScript = labelInfo.labelButtonScript;

        localHeight -= labelInfo.Height;

        /// Collectiong all the method button behaviours so that we can extract 
        /// all colorButton scripts from them.
        List<MethodButtonBehaviour> methodButtonBehs = new List<MethodButtonBehaviour>();

        /// All others are assigned to the parent so we can deactivate them seperately.
        /// We are doing to buttons on the same time, putting them side by side
        for (int i = 0; i < methods.Length; i += 2)
        {
            /// If we have unevent mothod count, we place the last button individualy to the left
            if (i == methods.Length - 1)
            {
                UiMethodNameWithParameters method = methods[i];
                Vector2 pos = new Vector2(marginX, localHeight);
                MethodButtonBehaviour beh = this.SpawnOneButton(pos, method.Name, monoName, localTransform);
                pos.y -= this.buttonY;
                float height = this.SpawnAllInputs(pos, beh, method, localTransform);
                localHeight -= (height + this.buttonY + this.marginY);
                methodButtonBehs.Add(beh);
            }
            /// Placing two buttons side by side.
            else
            {
                UiMethodNameWithParameters method1 = methods[i];
                UiMethodNameWithParameters method2 = methods[i + 1];

                Vector2 pos1 = new Vector2(marginX, localHeight);
                MethodButtonBehaviour beh1 = this.SpawnOneButton(pos1, method1.Name, monoName, localTransform);
                pos1.y -= this.buttonY;
                float height1 = this.SpawnAllInputs(pos1, beh1, method1, localTransform);

                Vector2 pos2 = new Vector2(marginX * 2 + this.buttonX, localHeight);
                MethodButtonBehaviour beh2 = this.SpawnOneButton(pos2, method2.Name, monoName, localTransform);
                pos2.y -= this.buttonY;
                float height2 = this.SpawnAllInputs(pos2, beh2, method2, localTransform);

                localHeight -= (Mathf.Max(height1, height2) + buttonY + marginY);

                methodButtonBehs.Add(beh1);
                methodButtonBehs.Add(beh2);
            }
        }

        ///Collecting all the color scripts so we can close Color Picker on colapse
        foreach (MethodButtonBehaviour methodScript in methodButtonBehs)
        {
            foreach (ParameterNameWithColorButtonScript colorButtonScript in methodScript.ColorParamaters)
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

        button.GetComponent<Image>().color = Colors.ActionButtonColor;

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

        ///Registering with InputFocusManager
        this.inputFocusManager.Register(inputField);

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

    #region }
}
#endregion