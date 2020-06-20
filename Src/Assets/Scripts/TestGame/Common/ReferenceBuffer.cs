using TMPro;
using UnityEngine;

public class ReferenceBuffer : MonoBehaviour
{
    public TMP_InputField TextEditorInputField { get; set; }
    public ShowActionsBehaviour ShowActions { get; set; }
    public ShowAvailableCSFiles ShowAvailableCSFiles { get; set; }
    public GameObject ColorPicker { get; set; }
    public PlayerHandling PlayerHandling { get; set; }
    public GameObject InfoTextObject { get; set; }
    public GameObject InfoTextCanvasGroup { get; set; }

    private void Awake()
    {
        this.TextEditorInputField = GameObject.Find("TextEditor").GetComponent<TMP_InputField>();
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
