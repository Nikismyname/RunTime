using System.Collections.Generic;
using UnityEngine;

public class TrackManager
{
    private const float distanceBetweenSpheres = 40;
    private Vector3? previousLocation;
    private GameObject ship;
    private List<float> collectionTimes;

    public TrackManager(GameObject ship)
    {
        this.collectionTimes = new List<float>(); 
        this.ship = ship;
        previousLocation = null;
    }

    public void StartRace()
    {
        CreateSphere();
    }

    public void SphereCollcted(float time)
    {
        Debug.Log(time);
        this.collectionTimes.Add(time); 
        CreateSphere();
    }

    public void CreateSphere()
    {
        var sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph.transform.localScale = new Vector3(10,10,10);
        sph.GetComponent<Renderer>().material.color = Color.red;

        var newLocation = Random.insideUnitSphere * distanceBetweenSpheres; 

        if(this.previousLocation != null)
        {
            newLocation = newLocation + previousLocation.Value; 
         
        }

        this.previousLocation = newLocation;

        sph.transform.position = newLocation;
        var script = sph.AddComponent<TrackSphere>();
        script.SetUp(this);

        sph.GetComponent<SphereCollider>().isTrigger = true;
    }
}
