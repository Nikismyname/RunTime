using UnityEngine;
using UnityEngine.EventSystems;

public class TargetBehaviour : MonoBehaviour,IPointerDownHandler
{
    public int id;
    private Main ms;
    private bool selected;
    Renderer myRenderer;
    Color originalColor;
    Color selectionColor = Color.green;

    private void Start()
    {
        ms = GameObject.Find("Main").GetComponent<Main>();
        this.selected = false;
        this.myRenderer = gameObject.GetComponent<Renderer>();
        this.originalColor = myRenderer.material.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.selected == true)
        {
            return;
        }

        ms.RegisterSelection(this.id);
    }

    public void SetUp(int id)
    {
        this.id = id;
    }

    public void VisualiseSelection(int id)
    {
        if (this.id == id)
        {
            this.selected = true;
            this.PaintSelected();
        }
        else if (this.selected == true)
        {
            this.selected = false;
            this.PaintUnselected();
        }
    }

    private void PaintSelected()
    {
        this.originalColor = myRenderer.material.color;
        this.myRenderer.material.color = this.selectionColor;
    }

    private void PaintUnselected()
    {
        if (this.myRenderer.material.color != this.selectionColor)
        {
            this.originalColor = this.myRenderer.material.color;
        }

        this.myRenderer.material.color = originalColor;
    }
}
