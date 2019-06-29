using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LabelButtonMonoName: MonoBehaviour
{
    private UiMonoGroupInformation monoGroup;
    private ManageActionsButtons manageButtons;
    public List<ColorSelectionButton> ColorSelectionButtons { get; set; } = new List<ColorSelectionButton>();
    private GameObject ColorPicker;

    private void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);

        this.ColorPicker = GameObject.Find("Main").GetComponent<ReferenceBuffer>().ColorPicker;
    }

    private void OnClick()
    {
        if (monoGroup.Collapsed) // Expand
        {
            monoGroup.Collapsed = false;
            manageButtons.DisplayInterfaceForTarger(null, true);
        }
        else // Colapse
        {
            monoGroup.Collapsed = true;
            manageButtons.DisplayInterfaceForTarger(null, true);
            var activeColorButtons = this.ColorSelectionButtons.Where(x => x.Active == true).ToArray();

            if(activeColorButtons.Length>0) { 
                this.ColorPicker.SetActive(false);

                foreach (var item in activeColorButtons)
                {
                    item.SetInactive(-1);
                }
            }
        }
    }

    public void SetUp(UiMonoGroupInformation monoGroup, ManageActionsButtons manageButtons)
    {
        this.monoGroup = monoGroup;
        this.manageButtons = manageButtons;
    }
}

