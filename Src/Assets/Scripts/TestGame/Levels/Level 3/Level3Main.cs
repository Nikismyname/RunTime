using System;
using UnityEngine;

public class Level3Main : MonoBehaviour, ILevelMain
{
    Main ms;

    private bool gatheredTheSphere;
    private bool placedTHeSphere;
    private LevelManager lm;

    private GameObject actor;
    private GameObject sphere;
    private GameObject finalPos;

    void Start()
    {
        var main = GameObject.Find("Main"); 
        this.lm = main.GetComponent<LevelManager>();
        var rb = main.GetComponent<ReferenceBuffer>(); 

        this.gatheredTheSphere = false;
        this.placedTHeSphere = false;

        this.ms = gameObject.GetComponent<Main>();
        var gl = new GenerateLevel(this.ms, rb);

        gl.CylinderBasePrefab();

        var target = gl.GenerateEntity(
            EntityType.Target,
            new Vector3(7, 0, 0),
            PrimitiveType.Sphere,
            Color.white,
            null, "Level3Actor123");

        var toTransport = gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(-7, 0, 0),
            PrimitiveType.Sphere,
            Color.red,
            null, "Level3ToTranport567"
        );

        var transportPosition = gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(5, 0, 5),
            PrimitiveType.Cube,
            Color.gray,
            null, "Level3FinalPosition789"
        );
    }

    public void ResetLevel()
    {
        if (this.gatheredTheSphere == false)
        {
            if ((this.actor.transform.position - this.sphere.transform.position).magnitude < 1)
            {
                this.gatheredTheSphere = true;
                this.sphere.SetActive(false);
            }
        }
        else if(this.placedTHeSphere == false)
        {
            if ((this.actor.transform.position - this.sphere.transform.position).magnitude < 1)
            {
                this.placedTHeSphere = true;
                this.lm.Success();
            }
        }
    }
}
