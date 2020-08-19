using Boo.Lang;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowPresetsBehaviour : MonoBehaviour
{
    private bool show;
    private List<ActionButton> buttons;

    void Start()
    {
        this.show = true;
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(MainOnClick);

        this.MapConnections();
    }

    public void MapConnections()
    {
        var existingMappings = ActionKeyPersistance.GetKeyCubeMapping();
        this.buttons = new List<ActionButton>();
            
        for (int i = 1; i <= 9; i++)
        {
            ActionButton btnInfo = new ActionButton();
            this.buttons.Add(btnInfo);
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
    }
    
    public void RedoMappingConnections()
    {
        var existingMappings = ActionKeyPersistance.GetKeyCubeMapping();
            
        for (int i = 0; i < 9; i++)
        {
            var btnInfo = buttons[i];
            int index = i;
            Button button = btnInfo.Button.GetComponent<Button>(); 
            button.onClick.RemoveAllListeners();
            btnInfo.Button.GetComponent<Button>().onClick.AddListener(() => ActionButtonOnClick(index+1));
            btnInfo.Button.SetActive(false);
            var mappings = existingMappings.Where(x => x.KeyId == i+1).ToArray();
            if (mappings.Length == 1)
            {
                btnInfo.CubeName = mappings[0].CubeName;
                btnInfo.Button.transform.Find("Text").GetComponent<Text>().text = mappings[0].CubeName;
            }
        }
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

            Vector3? droneMarker = null;
            if (ReferenceBuffer.Instance.DroneIntersection != null)
            {
                droneMarker = ReferenceBuffer.Instance.DroneIntersection.GetIntersectionPosition;
            }

            ReferenceBuffer.Instance.worldSpaceUI.connTracker.RunCube(btnInfo.CubeName, new Variable[]
            {
                new Variable
                {
                    Name = ResultCanvas.PlayerForwardVarName,
                    Value = ReferenceBuffer.Instance.PlayerObject.transform.forward,
                },
                new Variable
                {
                    Name = ResultCanvas.PlayerPositionVarName,
                    Value = ReferenceBuffer.Instance.PlayerObject.transform.position,
                },

                new Variable
                {
                    Name = ResultCanvas.PlayerMarkerVarName,
                    Value = ReferenceBuffer.Instance.PlayerIntersection.GetIntersectionPosition,
                },
                new Variable
                {
                    Name = ResultCanvas.DroneMarkerVarName,
                    Value = droneMarker,
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(6);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(7);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(8);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.ActionButtonOnClick(9);
        }
    }
}