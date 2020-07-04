using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Aattached to ShowActionsButton
/// </summary>
public class ShowActionsBehaviour : MonoBehaviour
{
    private GameObject actions;
    private bool shouldShow = true;
    private ShowAvailableCSFiles showFiles;
    private GameObject colorPicker; 

    void Start()
    {
        this.actions = GameObject.Find("ScrollableActions");
        actions.SetActive(false);
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        var referenceBuffer = GameObject.Find("Main").GetComponent<ReferenceBuffer>();
        this.showFiles = referenceBuffer.ShowAvailableCSFiles;
        this.colorPicker = referenceBuffer.ColorPicker;
    }

    public void Close()
    {
        this.DoClose();
    }

    public void Open()
    {
        this.DoOpen();
    }

    private void OnClick()
    {
        if (this.shouldShow) // Open
        {
            this.DoOpen();
        }
        else // Close
        {
            this.DoClose();
        }
    }

    private void DoClose()
    {
        this.actions.SetActive(false);
        this.shouldShow = true;
        this.colorPicker.SetActive(false);
    }

    private void DoOpen()
    {
        this.showFiles.Close();
        this.actions.SetActive(true);
        this.shouldShow = false;
    }
}
