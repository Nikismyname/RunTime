using System;
using System.IO;
using UnityEngine;

public class LevelPCTMain : LevelBase
{
    private GameObject shipPrefab;
    private GameObject player;
    private GameObject mainCamera;
    private Main ms;
    private GenerateLevel gl;
    private GameObject targetSphere;

    private void Start()
    {
        var main = GameObject.Find("Main");
        this.ms = main.GetComponent<Main>();
        this.shipPrefab = ms.shipPrefab;
        var rb = main.GetComponent<ReferenceBuffer>();
        this.gl = new GenerateLevel(this.ms, rb);

        ///Generate the environment
        this.gl.CylinderBasePrefab(new Vector3(40, 1, 40), true);
        ///...

        /// Player!
        this.player = this.gl.Player(new Vector3(0, 0, 0), true, true, true);
        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;
        ///...

        //GameObject.Find("ScrollableInfoText").SetActive(false);

        this.GenerateEntites();

        string text = File.ReadAllText(@"C:\Users\ASUS G751JY\source\repos\11111 Unity\UserCode\UserLevel\UserLevelMain.cs");
        _ = ReferenceBuffer.Instance.capp.ApplyToSelectedTarget(text, this.targetSphere);
    }

    private void GenerateEntites()
    {
        var nextLevel = this.gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(0, 0, 0), PrimitiveType.Sphere,
            Color.red,
            null,
            "NextLevelSphere",
            new Type[] { typeof(NextLevelSphere) });

        this.targetSphere = this.gl.GenerateEntity(
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
        ms.RegisterTarget(solvingCube, TargetType.Test, TestMap.ReverseLinkedListName);
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
        this.gl.AddEditableScriptToEntity<TeleportationBehaviour>(teleportationSphere, TeleportationBehaviour.Source);
    }
}
