using Boo.Lang;
using System;
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

    public ResultCanvas(GameObject prefab, Camera cam, ConnectionsTracker connTracker)
    {
        this.canvas = GameObject.Instantiate(prefab);
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
                this.variables[i].Name.text = vars[i].Name.Substring(0, 4);
                this.variables[i].Type .text = vars[i].Type.Name.Substring(0,4);
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
        object result = this.connTracker.PrintResult();
        this.resultText.text = result.ToString();
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

    public class VariableInput
    {
        public Type Type { get; set; }

        public string  Name { get; set; }

        public VariableInput(Type type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}



