﻿using UnityEngine;
using UnityEngine.UI;

public class ReferenceBuffer : MonoBehaviour
{
    public InputField TextEditorInputField { get; set; }
    public ShowActionsBehaviour ShowActions { get; set; }
    public ShowAvailableCSFiles ShowAvailableCSFiles { get; set; }
    public GameObject ColorPicker { get; set; }
    public PlayerHandling PlayerHandling { get; set; }
    public GameObject InfoTextObject { get; set; }
    public GameObject InfoTextCanvasGroup { get; set; }

    private void Awake()
    {
        this.TextEditorInputField = GameObject.Find("TextEditor").GetComponent<InputField>();
        this.ColorPicker = GameObject.Find("ColorPicker");
        this.ColorPicker.SetActive(false);
        this.ShowActions = GameObject.Find("ShowActionsButton").GetComponent<ShowActionsBehaviour>();
        this.ShowAvailableCSFiles = GameObject.Find("ShowAvailableFilesButton").GetComponent<ShowAvailableCSFiles>();
        this.InfoTextObject = GameObject.Find("InfoText");
        this.InfoTextCanvasGroup = GameObject.Find("ScrollableInfoText");
    }

    public void RegisterPlayerHandling (PlayerHandling playerHandling)
    {
        this.PlayerHandling = playerHandling; 
    }
}
