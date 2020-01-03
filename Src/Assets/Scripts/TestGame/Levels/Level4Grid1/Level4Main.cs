using System;
using UnityEngine;

public class Level4Main : MonoBehaviour, ILevelMain
{
    private GameObject player; 

    private void Start()
    {
        var main = GameObject.Find("Main");
        var ms = main.GetComponent<Main>();
        var gm = main.GetComponent<GridManager>();
        var buffer = main.GetComponent<ReferenceBuffer>(); 
        var gl = new GenerateLevel(ms,buffer,gm);
        var grid = gl.GenerateGrid(10,10);

        ///PLAYER
        this.player = gl.Player(new Vector3(20, 0, 10), true, true, true);
        var mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = mainCamera.GetComponent<CamHandling>();
        camHandling.target = player.transform;

        var solvingSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var tb = solvingSphere.AddComponent<TargetBehaviour>();
        ms.RegisterTarget(solvingSphere, TargetType.Test, GridTestMap.Level4TestName);
        solvingSphere.name = "Solving Sphere";
        solvingSphere.transform.position = new Vector3(0, 10, 0);
    }

    public void ResetLevel()
    {
        this.player.transform.position = new Vector3(0, 1, 0);
    }
}

