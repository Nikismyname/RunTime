using UnityEngine;

public class TrackSphere: MonoBehaviour
{
    private TrackManager trackManager;
    private float timeToCllect = 0f;

    public void SetUp(TrackManager trackManager)
    {
        this.trackManager = trackManager;
    }

    private void Update()
    {
        this.timeToCllect += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger: "+ other.gameObject.name);
        this.trackManager.SphereCollcted(this.timeToCllect);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collistion " + collision.gameObject.name);
        this.trackManager.SphereCollcted(this.timeToCllect);
        Destroy(gameObject);
    }
}

