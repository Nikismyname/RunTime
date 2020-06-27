using UnityEngine;
using UnityEngine.EventSystems;

public class ContextBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private string contextText = string.Empty;

    public void SetContextText(string contextText)
    {
        this.contextText = contextText;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ReferenceBuffer.Instance.UniUIManager.TargetTextOpen();
        ReferenceBuffer.Instance.UniUIManager.SetTargetText(this.contextText);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReferenceBuffer.Instance.UniUIManager.TargetTextClose();
    }
}

