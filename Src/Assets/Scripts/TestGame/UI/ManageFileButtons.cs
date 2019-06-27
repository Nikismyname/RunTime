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
    string[] filePaths; 

    private bool startExecuted = false;

    void Start()
    {
        this.parentTransform = new GameObject("TestParent");
        parentTransform.transform.SetParent(gameObject.transform, false);
        parentTransform.transform.position = gameObject.transform.position;

        this.marginY = 20;
        this.marginX = 20;
        this.xLength = gameObject.GetComponent<RectTransform>().sizeDelta.x;
        this.buttonX = xLength - 2 * marginX;
        this.buttonY = this.buttonX / 7;

        this.parentRT = gameObject.GetComponent<RectTransform>(); 

        //this.buttonPreFab.GetComponent<RectTransform>().sizeDelta = new Vector2(this.buttonX, this.buttonY);

        this.buttons = new List<GameObject>();
        this.startExecuted = true;
        this.OnEnable();
    }

    private void OnEnable()
    {
        if (this.startExecuted == false)
        {
            return;
        }

        this.filePaths = Compilation.GetAllCsFiles(Compilation.path);
        this.buttons.Clear();
        this.DrawFiles();
    }

    private void SpawnOneButton(Vector2 pos, string path)
    {
        var button = Instantiate(this.buttonPreFab, this.parentTransform.transform);
        
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(this.buttonX, this.buttonY);

        var position = new Vector2(pos.x + this.buttonX / 2, pos.y - this.buttonY / 2);
        button.GetComponent<RectTransform>().anchoredPosition = position;
        button.GetComponentInChildren<Text>().text = this.GetName(path);

        var script = button.AddComponent<FileSelectionButton>();
        script.SetUp(path);

        this.buttons.Add(button);
    }

    private string GetName(string path)
    {
        var tokens = path.Split(new char[] {'\\','/'}, StringSplitOptions.RemoveEmptyEntries);
        var namePart = tokens[tokens.Length - 1];
        namePart = namePart.Substring(0,namePart.Length-3);
        return namePart;
    }

    private float CalculateNewStartY()
    {
        return -(this.buttons.Count * (this.buttonY + this.marginY) + this.marginY);
    }

    private void DrawFiles()
    {
        var Y = this.CalculateNewStartY(); 

        foreach (var file in this.filePaths)
        {
            this.SpawnOneButton(new Vector2(this.marginX, Y), file);
            Y -= (buttonY + marginY);
        }

        var y = parentRT.sizeDelta.y;
        var x = parentRT.sizeDelta.x;
        var myY = -this.CalculateNewStartY();

        //Debug.Log(y);
        //Debug.Log(myY);

        if (y < myY)
        {
            //Debug.Log("here");
            parentRT.sizeDelta = new Vector2(x, myY);
        }
    }
}
