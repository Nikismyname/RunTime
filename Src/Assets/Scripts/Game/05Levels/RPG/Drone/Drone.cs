using UnityEngine;

public class Drone : MonoBehaviour
{
    private Camera myCamera;
    private GameObject indicator;
    private CamCenterIntersection camIntersect;

    private void Start()
    {
        this.myCamera = gameObject.GetComponent<Camera>();
        this.camIntersect = new CamCenterIntersection(this.gameObject, this.myCamera);
        ReferenceBuffer.Instance.RegisterDroneIntersection(this.camIntersect);
    }

    private void Update()
    {
        this.camIntersect.Update();
    }
}

