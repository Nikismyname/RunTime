using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ManageFileButtons : MonoBehaviour
{
    public GameObject buttonPreFab;
    private GameObject parentTransform;
    private float marginX;
    private float marginY;
    private float xLength;
    private float buttonX;
    private float buttonY;
    RectTransform parentRT;
    private Main ms;
    List<GameObject> buttons;

    private string[] folderPaths = new string[] { Compilation.path, Compilation.testPath };
    private string folderPath = "not set";
    private int folderPathIndex = 0;

    private bool startExecuted = false;

    void Start()
    {
        this.folderPath = this.folderPaths[this.folderPathIndex];

        this.parentTransform = new GameObject("TestParent");
        parentTransform.transform.SetParent(gameObject.transform, false);
        parentTransform.transform.position = gameObject.transform.position;

        this.marginY = 20;
        this.marginX = 20;
        this.xLength = gameObject.GetComponent<RectTransform>().sizeDelta.x;
        this.buttonX = xLength - 2 * marginX;
        this.buttonY = this.buttonX / 7;

        this.parentRT = gameObject.GetComponent<RectTransform>();

        this.buttons = new List<GameObject>();
        this.startExecuted = true;
        this.OnEnable();

        var btn = this.SpawnOneButton(new Vector2(this.marginX, -marginY), "SWITCH", false);
        var scr = btn.AddComponent<FileButtonSwitch>();
        btn.GetComponent<Image>().color = new Color32(229, 200, 25, 255);
        scr.SetUp(this);
    }

    private void OnEnable()
    {
        if (this.startExecuted == false)
        {
            return;
        }

        Redraw();
    }

    public void Redraw()
    {
        foreach (var btn in this.buttons)
        {
            Destroy(btn);
        }

        this.buttons.Clear();
        this.DrawFiles(Compilation.GetAllCsFiles(this.folderPath));
    }

    public void Switch()
    {
        this.folderPathIndex++;
        if (this.folderPathIndex == this.folderPaths.Length)
        {
            this.folderPathIndex = 0;
        }

        this.folderPath = this.folderPaths[this.folderPathIndex];

        Redraw();
    }

    private GameObject SpawnOneButton(Vector2 pos, string path, bool isPath = true)
    {
        var button = Instantiate(this.buttonPreFab, this.parentTransform.transform);

        button.GetComponent<RectTransform>().sizeDelta = new Vector2(this.buttonX, this.buttonY);

        var position = new Vector2(pos.x + this.buttonX / 2, pos.y - this.buttonY / 2);
        button.GetComponent<RectTransform>().anchoredPosition = position;
        if (isPath)
        {
            button.GetComponentInChildren<Text>().text = this.GetName(path);
            var script = button.AddComponent<FileSelectionButton>();
            script.SetUp(path);
            this.buttons.Add(button);
        }
        else
        {
            button.GetComponentInChildren<Text>().text = path;
        }

        return button;
    }

    private string GetName(string path)
    {
        var tokens = path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        var namePart = tokens[tokens.Length - 1];
        namePart = namePart.Substring(0, namePart.Length - 3);
        return namePart;
    }

    private float CalculateNewStartY()
    {
        return -(this.buttons.Count * (this.buttonY + this.marginY) + this.marginY + buttonY + marginY);
    }

    private void DrawFiles(string[] files)
    {
        var Y = this.CalculateNewStartY();

        foreach (var file in files)
        {
            this.SpawnOneButton(new Vector2(this.marginX, Y), file);
            Y -= buttonY + marginY;
        }

        var y = parentRT.sizeDelta.y;
        var x = parentRT.sizeDelta.x;
        var myY = -this.CalculateNewStartY();

        if (y < myY)
        {
            parentRT.sizeDelta = new Vector2(x, myY);
        }
    }
}
