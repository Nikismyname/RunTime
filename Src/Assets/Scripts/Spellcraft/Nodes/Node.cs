using UnityEngine;

public class Node : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private Camera myCamera;
    private float? currentPositionYPrev = null;

    private void Start()
    {
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }
    void OnMouseDown()
    {
        this.screenPoint = this.myCamera.WorldToScreenPoint(gameObject.transform.position);
        this.offset = gameObject.transform.position - this.myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        this.currentPositionYPrev = null;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = this.myCamera.ScreenToWorldPoint(curScreenPoint) + offset;
        Vector3 temp = gameObject.transform.position;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (currentPositionYPrev != null && currentPositionYPrev != curPosition.y)
            {
                float yoff = curPosition.y - currentPositionYPrev.Value;
                temp.z = temp.z + yoff;
            }
            currentPositionYPrev = curPosition.y;
        }
        else
        {
            temp.x = curPosition.x;
            temp.y = curPosition.y;
        }

        gameObject.transform.position = temp;
    }
}
