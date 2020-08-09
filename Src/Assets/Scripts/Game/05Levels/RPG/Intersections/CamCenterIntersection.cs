using UnityEngine;

public class CamCenterIntersection
{
    private Camera myCamera;
    private GameObject indicator;
    private GameObject gameObject;
    private bool active = true;
    private Vector3 viewport; 

    public CamCenterIntersection(GameObject go, Camera camera, Vector3? viewport = null)
    {
        this.viewport = viewport == null ? new Vector3(0.5f, 0.5f, 0.5f) : viewport.Value; 

        this.gameObject = go;
        this.myCamera = camera;
        this.indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        this.indicator.SetActive(false);
        this.indicator.name = "Drone Indicator";
        this.indicator.GetComponent<Renderer>().material.color = Color.red;
        GameObject.Destroy(this.indicator.GetComponent<Collider>());
    }

    public void Update()
    {
        if (this.myCamera.enabled == false)
        {
            return;
        }

        Ray ray = this.myCamera.ViewportPointToRay(this.viewport);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (this.active)
            {
                this.indicator.SetActive(true);
            }
            else
            {
                this.indicator.SetActive(false);
            }

            //Debug.Log("Hit!");

            this.indicator.transform.position = hit.point;
        }
        else
        {
            this.indicator.SetActive(false);
            //Debug.Log("No Hit!");
        }
    }

    public void ToggleActive()
    {
        if (this.active)
        {
            this.active = false;
        }
        else
        {
            this.active = true;
        }
    }

    public Vector3? GetIntersectionPosition => this.indicator.activeSelf == true ? this.indicator.transform.position : (Vector3?)null;

    public GameObject GetInterseciton => this.indicator;
}

