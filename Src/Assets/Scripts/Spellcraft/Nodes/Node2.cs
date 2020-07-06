using UnityEngine;

public class Node2 : MonoBehaviour
{
    private Camera myCamera;
    private Vector3 worldOffset;
    private Plane interactionPlane;
    private float? currentPositionYPrev = null;
    private GameObject[] shadows = new GameObject[6];

    private void Start()
    {
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();

        GameObject bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottom.transform.localScale = new Vector3(1, 0.1f, 1);
        bottom.SetActive(false);
        shadows[0] = bottom;

        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.transform.localScale = new Vector3(1, 0.1f, 1);
        top.SetActive(false);
        shadows[1] = top;

        GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
        left.transform.localScale = new Vector3(0.1f, 1, 1);
        left.SetActive(false);
        shadows[2] = left;

        GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
        right.transform.localScale = new Vector3(0.1f, 1, 1);
        right.SetActive(false);
        shadows[3] = right;

        GameObject toUs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        toUs.transform.localScale = new Vector3(1, 1, 0.1f);
        toUs.SetActive(false);
        shadows[4] = toUs;

        GameObject away = GameObject.CreatePrimitive(PrimitiveType.Cube);
        away.transform.localScale = new Vector3(1, 1, 0.1f);
        away.SetActive(false);
        shadows[5] = away;
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

            foreach (var item in this.shadows)
            {
                item.SetActive(true);
            }
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

            Vector3 temp = gameObject.transform.position;

            this.shadows[0].transform.position = new Vector3(temp.x, -SpellcraftConstants.HalfSize, temp.z);
            this.shadows[1].transform.position = new Vector3(temp.x, +SpellcraftConstants.HalfSize, temp.z);

            this.shadows[2].transform.position = new Vector3(-SpellcraftConstants.HalfSize, temp.y, temp.z);
            this.shadows[3].transform.position = new Vector3(+SpellcraftConstants.HalfSize, temp.y, temp.z);

            this.shadows[4].transform.position = new Vector3(temp.x, temp.y, -SpellcraftConstants.HalfSize);
            this.shadows[5].transform.position = new Vector3(temp.x, temp.y, +SpellcraftConstants.HalfSize);
        }
        else
        {
            Debug.LogError("Drag Bull");
        }
    }

    private void OnMouseUp()
    {
        foreach (var item in this.shadows)
        {
            item.SetActive(false);
        }
    }
}

