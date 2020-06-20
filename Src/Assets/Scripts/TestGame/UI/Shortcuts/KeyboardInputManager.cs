using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardInputManager: MonoBehaviour 
{
    private InputFocusManager focusManager;
    private ReferenceBuffer referenceBuffer;
    private ShowAvailableCSFiles fileSwitcher;
    private ShowActionsBehaviour actionSwitcher;
    private ShowCodeBehaviour showCode;
    private GameObject scrollableInfoText;

    private void Start()
    {
        var main = GameObject.Find("Main");
        this.focusManager = main.GetComponent<InputFocusManager>();
        this.referenceBuffer = main.GetComponent<ReferenceBuffer>();
        this.fileSwitcher = GameObject.Find("ShowAvailableFilesButton").GetComponent<ShowAvailableCSFiles>();
        this.actionSwitcher = GameObject.Find("ShowActionsButton").GetComponent<ShowActionsBehaviour>();

        this.showCode = GameObject.Find("ShowCodeButton").GetComponent<ShowCodeBehaviour>();
        this.scrollableInfoText = GameObject.Find("ScrollableInfoText");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && this.focusManager.SafeToTrigger())
        {
            if(this.referenceBuffer.InfoTextCanvasGroup.activeSelf == true)
            {
                this.referenceBuffer.InfoTextCanvasGroup.SetActive(false);
            }
            else
            {
                this.referenceBuffer.InfoTextCanvasGroup.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.fileSwitcher.Close();
            this.actionSwitcher.Close();
            /// Restoring to last nonEscape save! 
            this.showCode.ResoreToLast(); 
            this.showCode.Close();
            this.scrollableInfoText.SetActive(false);
        }

        /// Saving the state of the input field so that if escape is pressed after, we have the latest input! Because escape deletes all the current changes!
        if(Input.GetKeyDown(KeyCode.Escape) == false)
        {
            this.showCode.SaveLast();
        }
    }
}
