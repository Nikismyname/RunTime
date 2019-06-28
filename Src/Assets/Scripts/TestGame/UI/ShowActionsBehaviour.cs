using UnityEngine;
using UnityEngine.UI;

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

    private void OnClick()
    {
        if (this.shouldShow) // Open
        {
            this.showFiles.Close();
            this.actions.SetActive(true);
            this.shouldShow = false;
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
}
