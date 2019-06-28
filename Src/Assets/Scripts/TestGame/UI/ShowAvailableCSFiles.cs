using UnityEngine;
using UnityEngine.UI;

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

    private void OnClick()
    {
        if (shouldShow) // Open
        {
            this.showActions.Close();
            this.fileSelectionPanel.SetActive(true); 
            this.shouldShow = false;
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
}
