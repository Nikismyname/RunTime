#region INIT
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class ManageFileButtons : MonoBehaviour
{
    public GameObject buttonPreFab;
    private GameObject parentObject;
    private float marginX;
    private float marginY;
    private float xLength;
    private float buttonX;
    private float buttonY;
    RectTransform parentRT;
    List<GameObject> buttons;

    private string[] folderPaths;
    private string folderPath = "not set";
    private int folderPathIndex = 0;

    private bool startExecuted = false;

    void Start()
    {
        string[] directories = Directory.GetDirectories(Compilation.BasePath);
        this.folderPaths = directories.Where(x=>x.Split(Path.DirectorySeparatorChar).Last().StartsWith("X")).ToArray(); 

        /// Getting the initial folder to display
        this.folderPath = this.folderPaths[this.folderPathIndex];

        /// Creating the parent object
        this.parentObject = new GameObject("Parent Object");
        /// Setting the panel as a parent of the parent object 
        parentObject.transform.SetParent(gameObject.transform, false);
        parentObject.transform.position = gameObject.transform.position;

        /// Seting the margins 
        this.marginY = 20;
        this.marginX = 20;
        /// Getting the horizontal length of the panel
        this.xLength = gameObject.GetComponent<RectTransform>().sizeDelta.x;
        /// Calculating the button dimentions
        this.buttonX = xLength - 2 * marginX;
        this.buttonY = this.buttonX / 7;

        /// Cashing the panel rect transform
        this.parentRT = gameObject.GetComponent<RectTransform>();

        this.buttons = new List<GameObject>();
        this.startExecuted = true;

        ///Creating the permenent switch button (switches folders with cs files inside)
        var switchButton = this.SpawnOneButton(new Vector2(this.marginX, -marginY), "SWITCH", false);
        var scr = switchButton.AddComponent<FileButtonSwitch>();
        switchButton.GetComponent<Image>().color = Colors.ActionButtonColor;
        scr.SetUp(this);
        ///...
        
        this.OnEnable();
    }

    private void OnEnable()
    {
        if (this.startExecuted == false)
        {
            return;
        }

        Redraw();
    }
    #endregion

    #region DRAW_FILES
    /// <summary>
    /// Creates the buttons for reading file from a path. 
    /// If the buttons get outside the bounds of box containing the 
    /// it extends the box.
    /// </summary>
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
    #endregion

    #region REDRAW
    /// <summary>
    /// Destroys all current buttons, then gathers the available cs file paths and draws them. 
    /// </summary>
    public void Redraw()
    {
        foreach (var btn in this.buttons)
        {
            Destroy(btn);
        }

        this.buttons.Clear();
        this.DrawFiles(GetAllCsFiles(this.folderPath));
    }
    #endregion

    #region SWITCH
    /// <summary>
    /// Switches between filder paths and creates the cs buttons for that path.
    /// </summary>
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
    #endregion

    #region SPAWN_ONE_BUTTON
    /// <summary>
    /// Instanciates a button, resizes it and and places it 
    /// in the right position. If a path to script is passed we add the functionality 
    /// to extract the file text on button click, else we just set the name an return it.
    /// </summary>
    /// <param name="pos">The upper left corner position we need the button at</param>
    /// <param name="path">Path to a script or button name</param>
    /// <param name="isPath">Weather it is a path to a script or button name</param>
    /// <returns></returns>
    private GameObject SpawnOneButton(Vector2 pos, string path, bool isPath = true)
    {
        /// spawnig from prefab with right parant
        var button = Instantiate(this.buttonPreFab, this.parentObject.transform);

        /// resizing the button
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(this.buttonX, this.buttonY);

        /// offseting the corner position to center position
        var position = new Vector2(pos.x + this.buttonX / 2, pos.y - this.buttonY / 2);
        button.GetComponent<RectTransform>().anchoredPosition = position;

        /// isPath means we recive a path to scripts, which sets up the text in the file to out editor on click
        if (isPath)
        {
            button.GetComponentInChildren<Text>().text = this.GetName(path);
            var script = button.AddComponent<FileSelectionButton>();
            script.SetUp(path);
            this.buttons.Add(button);
        }
        /// else it is a regular button with custom functionality set outside the method
        else
        {
            button.GetComponentInChildren<Text>().text = path;
        }

        button.GetComponent<Image>().color = Colors.CsFileButtonColor;

        return button;
    }
    #endregion

    #region HELPERS
    /// <summary>
    /// Extracts the name on a file from path.
    /// </summary>
    private string GetName(string path)
    {
        var tokens = path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        var namePart = tokens[tokens.Length - 1];
        namePart = namePart.Substring(0, namePart.Length - 3);
        return namePart;
    }

    /// <summary>
    /// Calculates the height of all drawn buttons. 
    /// </summary>
    private float CalculateNewStartY()
    {
        return -(this.buttons.Count * (this.buttonY + this.marginY) + this.marginY + buttonY + marginY);
    }
    #endregion

    #region GET_ALL_FILES

    /// <summary>
    /// Gets all cs files at a given path(the place where the user writes their code)
    /// </summary>
    public static string[] GetAllCsFiles(string path)
    {
        var result = new List<string>();

        var allFiles = Directory.GetFiles(path);

        foreach (var item in allFiles)
        {
            if (item.EndsWith(".cs"))
            {
                result.Add(item);
            }
        }

        return result.ToArray();
    }

    #endregion

    #region END BRACKET
}
#endregion
