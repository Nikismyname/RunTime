using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultCanvas
{
    private GameObject canvas;
    private List<VariableUIElements> variables = new List<VariableUIElements>();
    private Button doneButton;
    private TMP_Text resultText;
    private ConnectionsTracker connTracker;
    public const string PlayerPositionVarName = "plyPos";
    public const string PlayerForwardVarName = "plyFor";
    public const string DroneMarker = "marDro";
    public const string PlayerMarker = "marPla";
    
    public void Reset()
    {
        /// TODO:
    }

    public ResultCanvas(GameObject prefab, Camera cam, ConnectionsTracker connTracker, Transform parent)
    {
        this.canvas = GameObject.Instantiate(prefab);
        this.canvas.transform.SetParent(parent);
        this.canvas.GetComponent<Canvas>().worldCamera = cam;
        this.ParseElements();

        this.doneButton = this.canvas.transform.Find("DoneButton").GetComponent<Button>();
        this.doneButton.onClick.AddListener(this.OnClickDone);

        this.resultText = this.canvas.transform.Find("ResultText").GetComponent<TMP_Text>();
        canvas.transform.position = Vector3.zero;

        this.connTracker = connTracker;
    }

    public void SetVariables(VariableInput[] vars)
    {
        for (int i = 0; i < 3; i++)
        {
            /// We have var passed
            if(i <= vars.Length -1)
            {
                this.variables[i].Name.text = vars[i].Name;
                this.variables[i].Type .text = vars[i].Type.Name;
            }
            else /// we have ui variable (we have 3 for now) but no data for them
            {
                this.variables[i].Panel.SetActive(false);
            }
        }
    } 

    private void ParseElements()
    {
        for (int i = 1; i <= 3; i++)
        {
            Transform panel = canvas.transform.Find($"Panel{i}");
            this.variables.Add(new VariableUIElements(
                    panel.Find("Type").GetComponent<TMP_Text>(),
                    panel.Find("Name").GetComponent<TMP_Text>(),
                    panel.Find("Input").GetComponent<TMP_InputField>(),
                    panel.gameObject
                ));
        }
    }

    private void OnClickDone()
    {
        List<Variable> vars = new List<Variable>();

        for (int i = 0; i < this.variables.Count; i++)
        {
            var variable = variables[i];

            if(variable.Panel.activeSelf == false)
            {
                continue;
            }

            object value;

            /// Hardcoding player variables for now!
            if (variable.Name.text == PlayerPositionVarName)
            {
                value = new Vector3(0,0,0);
            }
            else if (variable.Name.text == PlayerForwardVarName)
            {
                value = new Vector3(0, 0, 1);
            }
            else
            {
                value = float.Parse(variable.Value.text);
            }

            vars.Add(new Variable
            {
                Name = variable.Name.text,
                Value = value
            });
        }

        object result = this.connTracker.PrintResult(null, vars.ToArray());
        if (result == null)
        {
            this.resultText.text = "VOID";
        }
        else
        {
            this.resultText.text = result.ToString();
        }
    }

    private class VariableUIElements
    {
        public TMP_Text Type { get; set; }

        public TMP_Text Name { get; set; }

        public TMP_InputField Value { get; set; }

        public GameObject Panel { get; set; }

        public VariableUIElements(TMP_Text type, TMP_Text name, TMP_InputField value, GameObject panel)
        {
            this.Type = type;
            this.Name = name;
            this.Value = value;
            this.Panel = panel;
        }
    }

    public void SetPosition(Vector3 position, Quaternion? rotation = null)
    {
        this.canvas.transform.position = position; 
        if(rotation != null)
        {
            this.canvas.transform.rotation = rotation.Value;
        }
    }

    /// <summary>
    /// 0.01 is the current!
    /// </summary>
    public void SetScale(Vector3 scale)
    {
        this.canvas.transform.localScale = scale;
    }

    public void Show()
    {
        if (this.canvas.activeSelf == true)
        {
            this.canvas.SetActive(false);
        }
        else
        {
            this.canvas.SetActive(true);
        }
    }

    public void Hide()
    {
        this.canvas.SetActive(false);
    }
    public class VariableInput
    {
        public Type Type { get; set; }

        public string Name { get; set; }

        public VariableInput(Type type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}



