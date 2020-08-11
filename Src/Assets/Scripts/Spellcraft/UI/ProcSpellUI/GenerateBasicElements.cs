using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenerateBasicElements
{
    private int buttonPixelsX = 200;
    private int buttonPixelsY = 30;
    private float yOffset = 5f;
    private float xOffset = 5f;

    private SpellcraftProcUI procUI;

    public GenerateBasicElements(SpellcraftProcUI procUI)
    {
        this.procUI = procUI;
    }
    
    public GameObject DrawButton(string text, Vector2 pos, Vector2? sizeDelta = null, Color? color = null)
    {
        sizeDelta = sizeDelta ?? new Vector2(this.buttonPixelsX, this.buttonPixelsY);

        GameObject button = GameObject.Instantiate(this.procUI.buttonPrefab, this.procUI.canvasGO.transform);
        RectTransform rt = button.GetComponent<RectTransform>();
        rt.sizeDelta = sizeDelta.Value;
        button.transform.position = new Vector3(pos.x + sizeDelta.Value.x / 2, pos.y - sizeDelta.Value.y / 2, 0);
        button.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        if (color != null)
        {
            button.GetComponent<Image>().color = color.Value;
        }

        return button;
    }

    public GameObject DrawText(Vector2 pos, string textInput, int fontSize)
    {
        GameObject text = GameObject.Instantiate(this.procUI.textPrefab, this.procUI.canvasGO.transform);
        RectTransform rt = text.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(this.buttonPixelsX, this.buttonPixelsY);
        text.transform.position =
            new Vector3(pos.x + (float) this.buttonPixelsX / 2, pos.y - (float) this.buttonPixelsY / 2, 0);
        TMP_Text t = text.GetComponent<TMP_Text>();
        t.text = textInput;
        t.fontSize = fontSize;
        t.alignment = TextAlignmentOptions.Center;

        return text;
    }

    public GameObject DrawInputMenu(Vector2 pos, int xx = 200, int yy = 30)
    {
        GameObject input = GameObject.Instantiate(this.procUI.inputFieldPrefab, this.procUI.canvasGO.transform);
        RectTransform rt = input.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(xx, yy);
        input.transform.position = new Vector3(pos.x + (float) xx / 2, pos.y - (float) yy / 2, 0);

        return input;
    }
}