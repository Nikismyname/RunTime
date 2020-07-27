using Boo.Lang;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowPresetsBehaviour : MonoBehaviour
{
    private bool show;
    List<ActionButton> buttons = new List<ActionButton>();

    void Start()
    {
        this.show = true;

        var existingMappings = ActionKeyPersistance.GetKeyCubeMapping(); 

        for (int i = 1; i <= 9; i++)
        {
            ActionButton btnInfo = new ActionButton();
            buttons.Add(btnInfo);
            btnInfo.Button = GameObject.Find("ActionButton" + i);
            int index = i;
            btnInfo.Button.GetComponent<Button>().onClick.AddListener(() => ActionButtonOnClick(index));
            btnInfo.Button.SetActive(false);
            var mappings = existingMappings.Where(x => x.KeyId == i).ToArray();
            if (mappings.Length == 1)
            {
                btnInfo.CubeName = mappings[0].CubeName;
                btnInfo.Button.transform.Find("Text").GetComponent<Text>().text = mappings[0].CubeName;
            }
            btnInfo.ID = i;
        }

        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(MainOnClick);
    }

    private void MainOnClick()
    {
        if (this.show)
        {
            this.show = false;
            foreach (var button in this.buttons)
            {
                button.Button.SetActive(true);
            }
        }
        else
        {
            this.show = true;
            foreach (var button in this.buttons)
            {
                button.Button.SetActive(false);
            }
        }
    }

    private void ActionButtonOnClick(int id)
    {
        ActionButton btnInfo = this.buttons.Single(x => x.ID == id); 

        if (btnInfo.CubeName != null)
        {
            Debug.Log($"WORKS=> {btnInfo.CubeName}");

            GameObject player = ReferenceBuffer.Instance.PlayerObject; 

            ReferenceBuffer.Instance.worldSpaceUI.connTracker.PrintResult(btnInfo.CubeName, new Variable[] 
            {
                new Variable
                {
                    Name = ResultCanvas.PlayerForwardVarName, 
                    Value = player.transform.forward, 
                },
                new Variable
                {
                    Name = ResultCanvas.PlayerPositionVarName,
                    Value = player.transform.position,
                },
            });

            return;
        }

        CubeInfo[] infos = CubePersistance.GetAllSavedCubes();

        if (infos.Length == 0)
        {
            Debug.Log("There are no saved QUBES!!!");
            return;
        }

        string lastName = infos.Last().Name;

        var persistedKeys = ActionKeyPersistance.GetKeyCubeMapping();

        if (persistedKeys.Select(x => x.CubeName).Contains(lastName))
        {
            Debug.Log("That spell name already keyd!");
            return; 
        }

        ActionKeyPersistance.Persist(new ActionKeyPersistance.ActionKeyPersistanceData
        {
            CubeName = lastName,
            KeyId = id,
        });

        btnInfo.Button.transform.Find("Text").GetComponent<Text>().text = lastName;
    }

    private class ActionButton
    {
        public int ID { get; set; }

        public GameObject Button { get; set; }

        public string CubeName { get; set; } = null;
    }
}
