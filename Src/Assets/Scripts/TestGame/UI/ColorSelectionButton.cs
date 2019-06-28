using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionButton : MonoBehaviour
{
    private GameObject colorPicker;
    private ColorPicker colorPickerScript; 
    public Color color = Color.white;
    private Image myImage;
    private int id;

    private bool active = false;
    private List<ColorSelectionButton> others; 

    private void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
        this.colorPicker = GameObject.Find("Main").GetComponent<ReferenceBuffer>().ColorPicker;
        this.colorPickerScript = colorPicker.GetComponent<ColorPicker>();
        var script = this.colorPicker.GetComponent<ColorPicker>();

        this.myImage = gameObject.GetComponent<Image>(); 

        script.onValueChanged.AddListener(color => {
            if(this.active == false)
            {
                return; 
            } 

            this.color = color;
            this.myImage.color = color;
        });
    }

    public void SetUp(List<ColorSelectionButton> others, int id)
    {
        this.others = others;
        this.id = id;
    }

    public void SetInactive( int  id)
    {
        if (this.id != id)
        {
            this.active = false;
        }
    }

    private void OnClick()
    {
        // if the color picker was closed externaly, click here should still activate colorPicker 
        if(colorPicker.activeSelf == false)
        {
            this.active = false;
        } 

        if (this.active == false)
        {
            //stoping all other collor picker buttons from interacting with the ColorPicker
            foreach (var item in others)
            {
                item.SetInactive(this.id);
            }

            //setting the collor picker to the last color this button was
            this.colorPickerScript.CurrentColor = this.color;

            this.colorPicker.SetActive(true);
            this.active = true;
        }
        else
        {
            this.colorPicker.SetActive(false);
            this.active = false;
        }
    }
}
