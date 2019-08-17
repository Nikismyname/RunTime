using UnityEngine;

public class KeyboardInputManager: MonoBehaviour 
{
    private InputFocusManager focusManager;
    private ReferenceBuffer referenceBuffer; 

    private void Start()
    {
        var main = GameObject.Find("Main");
        this.focusManager = main.GetComponent<InputFocusManager>();
        this.referenceBuffer = main.GetComponent<ReferenceBuffer>();
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
    }
}
