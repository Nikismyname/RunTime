using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    MeshRenderer myRenderer;

    private void Start()
    {
        this.myRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.ChangeColorOnCollision(Color.red, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        this.ChangeColorOnCollision(Color.green, collision);
    }

    private void ChangeColorOnCollision(Color color, Collision collision)
    {
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

