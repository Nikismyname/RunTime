using UnityEngine;
using UnityEngine.EventSystems;

//https://answers.unity.com/questions/1539189/check-if-mouse-is-over-ui.html
[RequireComponent(typeof(EventTrigger))]
public class FocusManager : MonoBehaviour
{
    private EventTrigger eventTrigger;
    private GameObject lastSelectedGameobject = null;
    public bool? IsCurrentlyOverUI { get; set; } = null;

    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry(); // Pointer Enter
            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);

            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry(); //Pointer Exit
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);
        }
    }

    public void EnterUI()
    {
        this.IsCurrentlyOverUI = true;
        if (this.lastSelectedGameobject != null)
        {
            EventSystem.current.SetSelectedGameObject(this.lastSelectedGameobject);
        }
    }

    public void ExitUI()
    {
        this.IsCurrentlyOverUI = false; 

        var currentSelected = EventSystem.current.currentSelectedGameObject;
        if(currentSelected != null)
        {
            this.lastSelectedGameobject = currentSelected;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}

