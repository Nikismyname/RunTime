using System;
using UnityEngine;

public class LevelPCTMain : MonoBehaviour, ILevelMain
{
    public GameObject shipPrefab;
    private GameObject player;
    private GameObject mainCamera;
    private Main ms;
    private GenerateLevel gl;

    private void Start()
    {
        var main = GameObject.Find("Main");
        this.ms = main.GetComponent<Main>();
        var rb = main.GetComponent<ReferenceBuffer>();
        this.gl = new GenerateLevel(this.ms, rb);

        ///Generate the environment
        this.gl.CylinderBasePrefab(new Vector3(150, 1, 150), true);

        ///PLAYER
        this.player = this.gl.Player(new Vector3(20, 0, 10), true, true, true);

        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;

        this.GenerateInGameInterface();
    }

    private void GenerateInGameInterface()
    {
        var nextLevel = this.gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(0, 0, 0), PrimitiveType.Sphere,
            Color.red,
            null,
            "NextLevelSphere",
            new Type[] { typeof(NextLevelSphere) });

        var targetSphere = this.gl.GenerateEntity(
            EntityType.Target,
            new Vector3(0, 2, 0),
            PrimitiveType.Sphere,
            Color.blue,
            new Vector3(3, 3, 3),
            "TargetSphere",
            new Type[0]
        );

        var targetSphere2 = this.gl.GenerateEntity(
            EntityType.Target,
            new Vector3(0, 5, 0),
            PrimitiveType.Sphere,
            Color.blue,
            new Vector3(3, 3, 3),
            "TargetSphere2",
            new Type[0]
        );

        var ship = this.gl.GenerateSpaceShip(
            EntityType.Target,
            shipPrefab,
            new Vector3(20, 0, 20),
            "Body",
            null,
            "space ship",
            new Type[] { typeof(NewtonianSpaceShipInterface) }
        );

        var solvingCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var tb = solvingCube.AddComponent<TargetBehaviour>();
        ms.RegisterTarget(solvingCube, TestMap.ReverseLinkedListName);
        solvingCube.name = "SolvingCube";
        solvingCube.transform.position = new Vector3(10, 10, 10);

        var teleportationSphere = this.gl.GenerateEntity(
            EntityType.Target,
            new Vector3(10, 5, 10),
            PrimitiveType.Sphere,
            Color.red,
            new Vector3(3, 3, 3),
            "TeleportationSphere",
            new Type[0]
        );
        var teleportationScript = teleportationSphere.AddComponent<TeleportationBehaviour>();
        var teleportationSphereFuncs = Compilation.GenerateAllMethodsFromMonoType(teleportationScript.GetType());  
        ms.AttachMono(teleportationSphereFuncs,false, teleportationSphere, false, teleportationScript); 
    }

    public void ResetLevel()
    {
        this.player.transform.position = new Vector3(0, 1, 0);
    }
}
