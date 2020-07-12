using System;
using UnityEngine;
using UnityEngine.UI;

public class ConstantNode : MonoBehaviour
{
    public WorldSpaceUI.ConstantElements elements;
    public ParameterNode paramNode = null;

    private WorldSpaceUI UI;
    private object value;
    private Color originalColor;
    private Color selectedColor = Color.gray;
    private Color paramSelectedColor = Color.green;
    private bool used = false;

    public void Setup(object value, WorldSpaceUI UI, WorldSpaceUI.ConstantElements elements)
    {
        this.value = value;
        this.UI = UI;
        this.elements = elements;
        this.originalColor = this.elements.Button.GetComponent<Image>().color;

        gameObject.GetComponent<Button>().onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        Debug.Log("HERE " + this.value.ToString());
        this.UI.RegisterConstantClick(this);
    }

    public object GetVal()
    {
        return this.value;
    }

    public void SetUsed(bool used, ParameterNode node)
    {
        this.used = used;
        this.paramNode = node;
        this.elements.Used = used;
    }

    public void FixColorBeforeShow(ParameterNode node)
    {
        if (this.used)
        {
            if (this.paramNode == node)
            {
                this.SetColor(this.paramSelectedColor);
            }
            else
            {
                this.SetColor(this.selectedColor);
            }
        }
        else
        {
            this.SetColor(this.originalColor);
        }
    }

    private void SetColor(Color color)
    {
        this.elements.Button.GetComponent<Image>().color = color;
    }
}

