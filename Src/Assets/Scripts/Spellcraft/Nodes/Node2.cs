using UnityEngine;

public class Node2 : MonoBehaviour
{
    private Camera myCamera;
    private Vector3 worldOffset;
    private Plane interactionPlane;
    private float? currentPositionYPrev = null;

    private void Start()
    {
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    void OnMouseDown()
    {
        if (Physics.Raycast(this.myCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
        {
            Vector3 hitPoint = raycastHit.point;
            Vector3 objectPoint = raycastHit.transform.position;
            this.worldOffset = objectPoint - hitPoint;
            this.interactionPlane = new Plane(this.myCamera.transform.forward, hitPoint);
            this.currentPositionYPrev = null;
        }
        else
        {
            Debug.LogError("Down Bull");
        }
    }

    void OnMouseDrag()
    {
        Ray ray = this.myCamera.ScreenPointToRay(Input.mousePosition);
        if (this.interactionPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (currentPositionYPrev != null && currentPositionYPrev != Input.mousePosition.y)
                {
                    gameObject.transform.position += (gameObject.transform.position - this.myCamera.transform.position).normalized * 10f * (Input.mousePosition.y - currentPositionYPrev.Value) * Time.deltaTime;
                }
                currentPositionYPrev = Input.mousePosition.y;
            }
            else
            {
                gameObject.transform.position = hitPoint + this.worldOffset;
            }
        }
        else
        {
            Debug.LogError("Drag Bull");
        }
    }
}

