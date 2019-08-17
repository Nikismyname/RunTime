using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    private MeshRenderer myRenderer;
    private bool selected = false;
    private int x=-1, y=-1; 

    private void Start()
    {
        this.myRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void SetUp(int y, int x)
    {
        this.x = x;
        this.y = y; 
    }

    public void Select()
    {
        this.selected = true;
        this.myRenderer.material.color = Colors.TileSelected; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.ChangeColorOnCollision(Colors.TileStepedOn, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        this.ChangeColorOnCollision(Colors.TileDefault, collision);
    }

    private void ChangeColorOnCollision(Color color, Collision collision)
    {
        if(this.selected == true)
        {
            return; 
        }
        
        if (collision.gameObject.name != "Player")
        {
            return; 
        }

        if(this.myRenderer == null)
        {
            return; 
        }

        this.myRenderer.material.color = color;
    }
}

