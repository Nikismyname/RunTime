using System;
using UnityEngine;

public class LevelPCTMain : MonoBehaviour, ILevelMain
{
    private float speed;
    private GameObject player;
    private GameObject mainCamera;
    private Camera cameraComponent;
    private Main ms;
    private GenerateLevel gl;

    private void Start()
    {
        this.ms = GameObject.Find("Main").GetComponent<Main>();
        this.gl = new GenerateLevel(this.ms);

        this.speed = ms.playerSpeed;

        ///Generate the environment
        this.gl.CylinderBasePrefab(true);

        ///PLAYER
        this.player = this.gl.Player(new Vector3(-1,0,0), true, true);

        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;
        this.cameraComponent = this.mainCamera.GetComponent<Camera>();

        this.GenerateInGameInterface();
    }

    private void GenerateInGameInterface()
    {
        var nextLevel = this.gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(0,0,0),PrimitiveType.Sphere,
            Color.red, 
            null, 
            "NextLevelSphere",
            new Type[] { typeof(NextLevelSphere)});

        var targetSphere = this.gl.GenerateEntity(
            EntityType.Target,
            new Vector3(0, 2, 0),
            PrimitiveType.Sphere,
            Color.blue,
            new Vector3(3,3,3),
            "TargetSphere",
            new Type[0]
        );
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            var direction = this.GenerateNormalisedForward();
            this.player.transform.position +=direction* speed / Time.deltaTime;
            this.player.transform.rotation = Quaternion.LookRotation(-direction);
        }

        if (Input.GetKey(KeyCode.S))
        {
            var direction = this.GenerateNormalisedForward();
            this.player.transform.position -= direction * speed / Time.deltaTime;
            this.player.transform.rotation = Quaternion.LookRotation(direction);
        }


        if (Input.GetKey(KeyCode.D))
        {
            var direction = this.GenerateNormalisedForward(true);
            this.player.transform.position -= direction * speed / Time.deltaTime;
            this.player.transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Input.GetKey(KeyCode.A))
        {
            var direction = this.GenerateNormalisedForward(true);
            this.player.transform.position += direction * speed / Time.deltaTime;
            this.player.transform.rotation = Quaternion.LookRotation(-direction);
        }
    }

    private Vector3 GenerateNormalisedForward(bool side = false)
    {
        var forward = this.cameraComponent.transform.forward.normalized;
        var vectorTwo = new Vector2(forward.x, forward.z);
        vectorTwo.Normalize();
        forward.Set(vectorTwo.x, 0, vectorTwo.y);

        if (side)
        {
            forward = Quaternion.AngleAxis(-90, Vector3.up) * forward;
        }
        //Debug.Log("M " + forward.magnitude);
        return forward;
    }

    public void ResetLevel()
    {
        this.player.transform.position = new Vector3(0, 1, 0);
    }
}
