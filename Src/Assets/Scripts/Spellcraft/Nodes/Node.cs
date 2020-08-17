#region INIT

using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    private WorldSpaceUI UI;
    private bool isRotating = false;
    private Camera myCamera;
    private SpellcraftCam camBehaviour;
    private Vector3 worldOffset;
    private Plane interactionPlane;
    private float? currentPositionYPrev = null;
    private GameObject[] shadows = new GameObject[6];
    private PlanesWithMovingAxis[] normals = new PlanesWithMovingAxis[6];
    private bool dragged = false;

    private void Start()
    {
        this.normals[0] = new PlanesWithMovingAxis(new Vector3(1, 0, 0), "yz");
        this.normals[1] = new PlanesWithMovingAxis(new Vector3(-1, 0, 0), "yz");
        this.normals[2] = new PlanesWithMovingAxis(new Vector3(0, 1, 0), "xz");
        this.normals[3] = new PlanesWithMovingAxis(new Vector3(0, -1, 0), "xz");
        this.normals[4] = new PlanesWithMovingAxis(new Vector3(0, 0, 1), "xy");
        this.normals[5] = new PlanesWithMovingAxis(new Vector3(0, 0, -1), "xy");

        this.interactionPlane = new Plane(normals[0].normal, this.gameObject.transform.position);

        this.myCamera = ReferenceBuffer.Instance.Camera;
        this.camBehaviour = this.myCamera.gameObject.GetComponent<SpellcraftCam>();

        GameObject bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottom.name = "Bottom Shadow";
        bottom.transform.SetParent(this.UI.spellcraftParent.transform);
        bottom.transform.localScale = new Vector3(1, 0.1f, 1);
        bottom.SetActive(false);
        shadows[0] = bottom;

        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "Top Shadow";
        top.transform.SetParent(this.UI.spellcraftParent.transform);
        top.transform.localScale = new Vector3(1, 0.1f, 1);
        top.SetActive(false);
        shadows[1] = top;

        GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
        left.name = "Left Shadow";
        left.transform.SetParent(this.UI.spellcraftParent.transform);
        left.transform.localScale = new Vector3(0.1f, 1, 1);
        left.SetActive(false);
        shadows[2] = left;

        GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
        right.name = "Right Shadow";
        right.transform.SetParent(this.UI.spellcraftParent.transform);
        right.transform.localScale = new Vector3(0.1f, 1, 1);
        right.SetActive(false);
        shadows[3] = right;

        GameObject toUs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        toUs.name = "ToUs Shadow";
        toUs.transform.SetParent(this.UI.spellcraftParent.transform);
        toUs.transform.localScale = new Vector3(1, 1, 0.1f);
        toUs.SetActive(false);
        shadows[4] = toUs;

        GameObject away = GameObject.CreatePrimitive(PrimitiveType.Cube);
        away.name = "Away Shadow";
        away.transform.SetParent(this.UI.spellcraftParent.transform);
        away.transform.localScale = new Vector3(1, 1, 0.1f);
        away.SetActive(false);
        shadows[5] = away;
    }

    public void Setup(WorldSpaceUI ui)
    {
        this.UI = ui;
    }

    #endregion

    #region MOUSE

    void OnMouseDown()
    {
        if (this.isRotating || this.UI.zoomMode == ZoomMode.ClassNodeZoom)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            this.camBehaviour.TriggerZoom(this.gameObject);
            this.UI.zoomMode = ZoomMode.ClassNodeZoom;
            return;
        }

        if (Physics.Raycast(this.myCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
        {
            Vector3 hitPoint = raycastHit.point;
            Vector3 objectPoint = this.gameObject.transform.position;
            this.worldOffset = objectPoint - hitPoint;
            this.currentPositionYPrev = null;
            this.SetShadowsActive(true);
            this.UI.SetDragged(this);
            this.dragged = true;
        }
        else
        {
            Debug.LogError("Down Bull");
        }
    }

    private void OnMouseUp()
    {
        this.SetShadowsActive(false);
    }

    void OnMouseDrag()
    {
        if (this.isRotating || this.dragged == false || this.UI.zoomMode == ZoomMode.ClassNodeZoom)
        {
            return;
        }

        Ray mouseRay = this.myCamera.ScreenPointToRay(Input.mousePosition);

        PlanesWithMovingAxis closestPlain = this.GetAppropriateAxis();

        if (closestPlain.normal != this.interactionPlane.normal)
        {
            this.interactionPlane = new Plane(closestPlain.normal, gameObject.transform.position);
        }

        if (this.interactionPlane.Raycast(mouseRay, out float enter))
        {
            Vector3 hitPoint = mouseRay.GetPoint(enter);
            Vector3 newLocation = hitPoint + this.worldOffset;

            hitPoint.DrawCrossUpdate(Color.red);
            newLocation.DrawCrossUpdate(Color.green);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (currentPositionYPrev != null && currentPositionYPrev != Input.mousePosition.y)
                {
                    Vector3 tempy = gameObject.transform.position;

                    float offset = (currentPositionYPrev.Value - Input.mousePosition.y) * Time.deltaTime * 3f;

                    if (closestPlain.axis.Contains('x') == false)
                    {
                        tempy.x += offset;
                    }
                    if (closestPlain.axis.Contains('y') == false)
                    {
                        tempy.y += offset;
                    }
                    if (closestPlain.axis.Contains('z') == false)
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

                if (closestPlain.axis.Contains('x'))
                {
                    tempy.x = newLocation.x;
                }
                if (closestPlain.axis.Contains('y'))
                {
                    tempy.y = newLocation.y;
                }
                if (closestPlain.axis.Contains('z'))
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

    #endregion

    #region HELPERS

    private PlanesWithMovingAxis GetAppropriateAxis()
    {
        PlanesWithMovingAxis[] mostFittingNormals = this.normals.Where(x => Vector3.Angle(x.normal, this.myCamera.transform.forward) == this.normals.Min(y => Vector3.Angle(y.normal, this.myCamera.transform.forward))).ToArray();
        return mostFittingNormals[0];
    }

    private class PlanesWithMovingAxis
    {
        public Vector3 normal { get; set; }
        public string axis { get; set; }

        public PlanesWithMovingAxis(Vector3 normal, string axis)
        {
            this.normal = normal;
            this.axis = axis;
        }
    }

    #endregion

    #region PUBLIC_INTERFACE

    private void SetShadowsActive(bool active)
    {
        foreach (var item in this.shadows)
        {
            item.SetActive(active);
        }
    }

    public void SetRotating(bool isRotating)
    {
        this.isRotating = isRotating;
        if (this.isRotating == true)
        {
            this.dragged = false;
        }
    }

    #endregion;

#region }
}
#endregion

