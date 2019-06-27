using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    public int id;
    private Main ms;
    private bool selected;
    Renderer myRenderer; 
    Color originalColor;

    private void Start()
    {
        ms = GameObject.Find("Main").GetComponent<Main>();
        this.selected = false;
        this.myRenderer = gameObject.GetComponent<Renderer>(); 
        this.originalColor = myRenderer.material.color;
    }

    private void OnMouseDown()
    {
        ms.RegisterSelection(this.id);
    }

    public void SetUp(int id)
    {
        this.id = id;
    }

    public void VisualiseSelection(int id)
    {
        if(this.id == id)
        {
            this.selected = true;
            this.PaintSelected();
        }
        else if(this.selected == true)
        {
            this.selected = false;
            this.PaintNotSelected();
        }
    }

    private void PaintSelected()
    {
        this.myRenderer.material.color = Color.green;
    }

    private void PaintNotSelected()
    {
        this.myRenderer.material.color = originalColor;
    }
}
