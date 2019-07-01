using UnityEngine;
using UnityEngine.UI;

public class FileButtonSwitch: MonoBehaviour
{
    private ManageFileButtons manageFileButtons;

    private void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
    }

    public void SetUp(ManageFileButtons mf)
    {
        this.manageFileButtons = mf; 
    }

    public void OnClick()
    {
        this.manageFileButtons.Switch();
    }
}

