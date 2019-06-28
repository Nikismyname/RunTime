using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MethodButtonBehaviour : MonoBehaviour
{
    private Main ms;
    private string mono = "default";
    private string method = "default";
    private bool isColor = false;

    private List<ParameterNameWithInputField> parameters = new List<ParameterNameWithInputField>();
    private List<ParameterNameWithColorButtonScript> colorParamaters = new List<ParameterNameWithColorButtonScript>();

    private ColorSelectionButton colorSelectionButtonScript = null;
    private string colorSelectionParamater = null;

    private void Start()
    {
        //this.parameters = new List<ParameterNameWithInputText>(); 

        this.ms = GameObject.Find("Main").GetComponent<Main>();

        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
    }

    public void SetUp(string mono, string method)
    {
        this.mono = mono;
        this.method = method;
    }

    public void RegisterNonColorParamater(string parameter, InputField inputField)
    {
        this.parameters.Add(new ParameterNameWithInputField
        {
            ParamaterName = parameter,
            InputField = inputField,
        });
    }

    public void RegisterColorParam(string paramater, ColorSelectionButton colorSelectionScript)
    {
        this.colorParamaters.Add(new ParameterNameWithColorButtonScript
        {
            ParamaterName = paramater,
            colorScript = colorSelectionScript,
        });
    }

    private void OnClick()
    {
        var para = new List<ParameterNameWithSingleObjectValues>();

        for (int i = 0; i < this.parameters.Count; i++)
        {
            var p = this.parameters[i];
            para.Add(new ParameterNameWithSingleObjectValues
            {
                ParameterName = p.ParamaterName,
                ParameterValue = p.InputField.text,
            });
        }

        for (int i = 0; i < this.colorParamaters.Count; i++)
        {
            var p = this.colorParamaters[i];
            para.Add(new ParameterNameWithSingleObjectValues
            {
                ParameterName = p.ParamaterName,
                ParameterValue = p.colorScript.color,
            });
        }

        this.ms.CallFunction(this.mono, this.method, para.ToArray());
    }
}