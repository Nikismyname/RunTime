using UnityEngine;
using UnityEngine.UI;

public class Test1ButtonBehaviour : MonoBehaviour
{
    private bool shouldShow = true;
    GameObject fileSelectionPanel;

    void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);

        this.fileSelectionPanel = GameObject.Find("ScrollableFileSelection");
        this.fileSelectionPanel.SetActive(false);
    }

    private void OnClick()
    {
        if (shouldShow)
        {
            this.fileSelectionPanel.SetActive(true); 
            this.shouldShow = false;
        }
        else
        {
            this.fileSelectionPanel.SetActive(false);
            this.shouldShow = true;
        }
    }
}
