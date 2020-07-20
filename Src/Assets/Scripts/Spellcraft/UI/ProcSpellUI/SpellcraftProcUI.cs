﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellcraftProcUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject inputFieldPrefab;
    public GameObject dropDownPrefab;
    public GameObject textPrefab;
    private Camera camera;

    private float canvasHalfY = 250;
    private float canvasHalfX = 500;

    private int buttonPixelsX = 200;
    private int buttonPixelsY = 30;

    private float globalScale = 0.2f;

    private float YOffset = 5f;
    private float XOffset = 5f;

    GameObject canvasGO;

    List<GameObject> UIElements = new List<GameObject>();

    private void Start()
    {
        this.camera = Camera.main;

        this.canvasGO = new GameObject("MainCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.worldCamera = camera;
        var rectT = canvas.GetComponent<RectTransform>();
        this.canvasGO.AddComponent<CanvasScaler>();
        this.canvasGO.AddComponent<GraphicRaycaster>();
        rectT.sizeDelta = new Vector2(this.canvasHalfX * 2, this.canvasHalfY * 2);

        this.CreateLoadRow(new Vector2(0, 0), out float x1, out float y1);
        this.DrawSaveCubeMenu(new Vector2(x1, 0));

        /// assuming  00 is TopRight so far, moving all elements to align
        foreach (var elem in this.UIElements)
        {
            elem.transform.position -= new Vector3(this.canvasHalfX, -this.canvasHalfY);
        }

        canvasGO.SetScale(new Vector3(this.globalScale, this.globalScale, this.globalScale));
    }

    private void SetUpLoadMenu()
    {

    }

    private void CreateLoadRow(Vector2 TR, out float x, out float y)
    {
        string[] realNames = CubePersistance.GetAllSavedCubes().Select(z=> z.Name).ToArray();

        string[] textNames = new string[] { "1", "2", "3", "4", "5" };

        for (int i = 0; i < textNames.Length; i++)
        {
            string name = textNames[i];

            this.DrawButton(name, new Vector2(TR.x, TR.y - i * (this.buttonPixelsY + this.YOffset)));
            this.DrawButton("X", new Vector2(TR.x + this.buttonPixelsX + this.XOffset, TR.y - i * (this.buttonPixelsY + this.YOffset)), new Vector2(30, 30), Color.red);
        }

        x = TR.x + this.buttonPixelsX + this.XOffset * 2 + 30;
        y = 42;
    }

    private void DrawSaveCubeMenu(Vector2 TR)
    {
        this.DrawText(new Vector2(TR.x, TR.y), "Name The Save", 20);
        this.DrawInputMenu(new Vector2(TR.x, TR.y - this.YOffset - this.buttonPixelsY));
        DrawButton("Save", new Vector2(TR.x, TR.y - (this.YOffset + this.buttonPixelsY) * 2));
    }

    #region PRIMITIVES

    private void DrawButton(string text, Vector2 pos, Vector2? sizeDelta = null, Color? color = null)
    {
        sizeDelta = sizeDelta == null ? new Vector2(this.buttonPixelsX, this.buttonPixelsY) : sizeDelta;

        GameObject button = Instantiate(this.buttonPrefab, canvasGO.transform);
        RectTransform rt = button.GetComponent<RectTransform>();
        rt.sizeDelta = sizeDelta.Value;
        button.transform.position = new Vector3(pos.x + sizeDelta.Value.x / 2, pos.y - sizeDelta.Value.y / 2, 0);
        button.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        if (color != null)
        {
            button.GetComponent<Image>().color = color.Value;
        }

        this.UIElements.Add(button);
    }

    private void DrawText(Vector2 pos, string textInpt, int fontSize, int XX = 200, int YY = 30)
    {
        GameObject text = Instantiate(this.textPrefab, canvasGO.transform);
        RectTransform rt = text.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(this.buttonPixelsX, this.buttonPixelsY);
        text.transform.position = new Vector3(pos.x + this.buttonPixelsX / 2, pos.y - this.buttonPixelsY / 2, 0);
        TMP_Text t = text.GetComponent<TMP_Text>();
        t.text = textInpt;
        t.fontSize = fontSize;
        t.alignment = TextAlignmentOptions.Center;

        this.UIElements.Add(text);
    }

    private void DrawInputMenu(Vector2 pos, int XX = 200, int YY = 30)
    {
        GameObject input = Instantiate(this.inputFieldPrefab, canvasGO.transform);
        RectTransform rt = input.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(XX, YY);
        input.transform.position = new Vector3(pos.x + XX / 2, pos.y - YY / 2, 0);

        this.UIElements.Add(input);
    }

    #endregion

    #region HELPERS

    #endregion
}

