using UnityEngine;

public class KeyboardInputManager: MonoBehaviour 
{
    private InputFocusManager focusManager;
    private ReferenceBuffer referenceBuffer;
    private ShowAvailableCSFiles fileSwitcher;
    private ShowActionsBehaviour actionSwitcher;

    private void Start()
    {
        var main = GameObject.Find("Main");
        this.focusManager = main.GetComponent<InputFocusManager>();
        this.referenceBuffer = main.GetComponent<ReferenceBuffer>();
        this.fileSwitcher = GameObject.Find("ShowAvailableFilesButton").GetComponent<ShowAvailableCSFiles>();
        this.actionSwitcher = GameObject.Find("ShowActionsButton").GetComponent<ShowActionsBehaviour>();
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
        }
    }
}
