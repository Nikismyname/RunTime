using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MethodButtonBehaviour : MonoBehaviour
{
    Main ms;
    private string mono = "default";
    private string method = "default";
    List<ParameterNameWithInputField> parameters = new List<ParameterNameWithInputField>();

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

    public void RegisterParamater(string parameter, InputField inputField)
    {
        this.parameters.Add(new ParameterNameWithInputField
        {
            InputField = inputField,
            ParamaterName = parameter
        });
    }

    private void OnClick()
    {
        var para = new ParameterNameWithSingleStringValue[this.parameters.Count];
        for (int i = 0; i < this.parameters.Count; i++)
        {
            var p = this.parameters[i];
            para[i] = new ParameterNameWithSingleStringValue
            {
                ParameterName = p.ParamaterName,
                ParameterValue = p.InputField.text,
            };
        }
        this.ms.CallFunction(this.mono, this.method, para);
    }
}