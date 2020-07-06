using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Camera myCamera;
    private Vector3 worldOffset;
    private Plane interactionPlane;
    private float? currentPositionYPrev = null;
    private GameObject[] shadows = new GameObject[6];
    private NormalToAxis[] normals = new NormalToAxis[6];

    private void Start()
    {
        this.normals[0] = new NormalToAxis(new Vector3(1, 0, 0), "yz");
        this.normals[1] = new NormalToAxis(new Vector3(-1, 0, 0), "yz");
        this.normals[2] = new NormalToAxis(new Vector3(0, 1, 0), "xz");
        this.normals[3] = new NormalToAxis(new Vector3(0, -1, 0), "xz");
        this.normals[4] = new NormalToAxis(new Vector3(0, 0, 1), "xy");
        this.normals[5] = new NormalToAxis(new Vector3(0, 0, -1), "xy");

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
        var some = GetAppropriateAxis(); 
        Plane some1 = new Plane(some.normal, gameObject.transform.position);

        if (some1.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 newLocation = hitPoint + this.worldOffset;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (currentPositionYPrev != null && currentPositionYPrev != Input.mousePosition.y)
                {
                    Vector3 tempy = gameObject.transform.position;

                    float offset = (currentPositionYPrev.Value - Input.mousePosition.y) * Time.deltaTime * 3f;

                    if (some.axis.Contains('x') == false)
                    {
                        tempy.x += offset;
                    }
                    if (some.axis.Contains('y') == false)
                    {
                        tempy.y += offset;
                    }
                    if (some.axis.Contains('z') == false)
                    {
                        tempy.z += offset;
                    }

                    gameObject.transform.position = tempy;
                }

                currentPositionYPrev = Input.mousePosition.y;
            }
            else ///Plane movement
            {
                Vector3 tempy = gameObject.transform.position;

                if (some.axis.Contains('x'))
                {
                    tempy.x = newLocation.x;
                }
                if (some.axis.Contains('y'))
                {
                    tempy.y = newLocation.y;
                }
                if (some.axis.Contains('z'))
                {
                    tempy.z = newLocation.z;
                }

                gameObject.transform.position = tempy;
            }

            #region SHADOWS

            Vector3 temp = gameObject.transform.position;

            this.shadows[0].transform.position = new Vector3(temp.x, -SpellcraftConstants.HalfSize, temp.z);
            this.shadows[1].transform.position = new Vector3(temp.x, +SpellcraftConstants.HalfSize, temp.z);

            this.shadows[2].transform.position = new Vector3(-SpellcraftConstants.HalfSize, temp.y, temp.z);
            this.shadows[3].transform.position = new Vector3(+SpellcraftConstants.HalfSize, temp.y, temp.z);

            this.shadows[4].transform.position = new Vector3(temp.x, temp.y, -SpellcraftConstants.HalfSize);
            this.shadows[5].transform.position = new Vector3(temp.x, temp.y, +SpellcraftConstants.HalfSize);

            #endregion
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
    private NormalToAxis GetAppropriateAxis()
    {
        NormalToAxis[] mostFittingNormals = this.normals.Where(x => Vector3.Angle(x.normal, this.myCamera.transform.forward) == this.normals.Min(y => Vector3.Angle(y.normal, this.myCamera.transform.forward))).ToArray();
        return mostFittingNormals[0];
    }

    private class NormalToAxis
    {
        public Vector3 normal { get; set; }
        public string  axis { get; set; }

        public NormalToAxis(Vector3 normal, string axis)
        {
            this.normal = normal;
            this.axis = axis;
        }
    }
}

