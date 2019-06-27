using UnityEngine;
using UnityEngine.UI;

public class LabelButtonMonoName: MonoBehaviour
{
    private UiMonoGroupInformation monoGroup;
    private ManageActionsButtons manageButtons;

    private void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        if (monoGroup.Collapsed)
        {
            //Debug.Log("Expanding");
            monoGroup.Collapsed = false;
            manageButtons.DisplayInterfaceForTarger(null, true);
        }
        else
        {
            //Debug.Log("Collapsing");
            monoGroup.Collapsed = true;
            manageButtons.DisplayInterfaceForTarger(null, true);
        }
    }

    public void SetUp(UiMonoGroupInformation monoGroup, ManageActionsButtons manageButtons)
    {
        this.monoGroup = monoGroup;
        this.manageButtons = manageButtons;
    }
}

