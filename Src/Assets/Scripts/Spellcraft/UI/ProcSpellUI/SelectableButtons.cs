using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableButtons
{
    private List<SelectableButton> buttons = new List<SelectableButton>();

    private Color notSelectedColor = Color.white;
    private Color selectedColor = Color.green;

    public void RegisterButton(Button button, Func<bool, bool> func)
    {
        SelectableButton sb = new SelectableButton(button);
        
        button.onClick.AddListener(() =>
        {
            bool isSelected = func(sb.IsSelected);
            sb.IsSelected = isSelected;
            this.FixColor(sb);
        });
    }

    private void FixColor(SelectableButton sb)
    {
        if (sb.IsSelected)
        {
            sb.Button.GetComponent<Image>().color = this.selectedColor;
        }
        else
        {
            sb.Button.GetComponent<Image>().color = this.notSelectedColor;
        }
    }

    private class SelectableButton
    {
        public SelectableButton(Button button)
        {
            this.Button = button;
            this.IsSelected = false;
        }

        public Button Button { get; }

        public bool IsSelected { get; set; }
    }
}