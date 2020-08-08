using UnityEngine;

public class Drone : MonoBehaviour
{
    private Camera myCamera;
    private GameObject indicator;

    private void Start()
    {
        this.myCamera = gameObject.GetComponent<Camera>();
        this.indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        this.indicator.SetActive(false);
        this.indicator.name = "Drone Indicator";
        this.indicator.GetComponent<Renderer>().material.color = Color.red;
        Destroy(this.indicator.GetComponent<Collider>());
    }

    private void Update()
    {
        if(myCamera.enabled == false)
        {
            return; 
        }

        Ray ray = this.myCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            this.indicator.SetActive(true); 

            this.indicator.transform.position = hit.point;
        }
        else
        {
            this.indicator.SetActive(false);
        }
    }
}

