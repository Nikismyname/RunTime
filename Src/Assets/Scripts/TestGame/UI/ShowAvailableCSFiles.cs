using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to ShowAvailableFilesButton
/// </summary>
public class ShowAvailableCSFiles : MonoBehaviour
{
    private bool shouldShow = true;
    GameObject fileSelectionPanel;
    private ShowActionsBehaviour showActions;

    void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);

        this.fileSelectionPanel = GameObject.Find("ScrollableFileSelection");
        this.fileSelectionPanel.SetActive(false);

        this.showActions = GameObject.Find("Main").GetComponent<ReferenceBuffer>().ShowActions;
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
        if (shouldShow) // Open
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
        this.fileSelectionPanel.SetActive(false);
        this.shouldShow = true;
    }

    private void DoOpen()
    {
        this.showActions.Close();
        this.fileSelectionPanel.SetActive(true);
        this.shouldShow = false;
    }
}
