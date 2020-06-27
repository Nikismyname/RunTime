using UnityEngine;

public class WallJumpMain : LevelBase
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
        this.gl.CylinderBasePrefab(new Vector3(150,1,150),true);

        ///PLAYER
        this.player = this.gl.Player(new Vector3(20, 0, 10), true, true, true);

        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;

        this.GenerateWalls();
    }

    private void GenerateWalls()
    {
        //var nextLevel = this.gl.GenerateEntity(
        //    EntityType.NonTarget,
        //    new Vector3(0, 0, 0), PrimitiveType.Sphere,
        //    Color.red,
        //    null,
        //    "NextLevelSphere",
        //    new Type[] { typeof(NextLevelSphere) });

        //var targetSphere = this.gl.GenerateEntity(
        //    EntityType.Target,
        //    new Vector3(0, 2, 0),
        //    PrimitiveType.Sphere,
        //    Color.blue,
        //    new Vector3(3, 3, 3),
        //    "TargetSphere",
        //    new Type[0]
        //);

        //var targetSphere2 = this.gl.GenerateEntity(
        //    EntityType.Target,
        //    new Vector3(0, 5, 0),
        //    PrimitiveType.Sphere,
        //    Color.blue,
        //    new Vector3(3, 3, 3),
        //    "TargetSphere2",
        //    new Type[0]
        //);

        //var ship = this.gl.GenerateSpaceShip(
        //    EntityType.Target,
        //    shipPrefab,
        //    new Vector3(20, 0, 20),
        //    "Body",
        //    null,
        //    "space ship",
        //    new Type[] { typeof(NewtonianSpaceShipInterface) }
        //);

        //var solvingCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //var tb = solvingCube.AddComponent<TargetBehaviour>();
        //ms.RegisterTarget(solvingCube, TestMap.ReverseLinkedListName);
        //solvingCube.name = "SolvingCube";
        //solvingCube.transform.position = new Vector3(10,10,10);

        //var wall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //wall1.tag = "Wall";
        //wall1.name = "Wall1";
        //wall1.transform.localScale = new Vector3(100,100, 1);
        //wall1.transform.rotation = Quaternion.Euler(0,75,0);
        //wall1.transform.position = new Vector3(0,0,9);

        //var wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //wall2.tag = "Wall";
        //wall2.name = "Wall2";
        //wall2.transform.localScale = new Vector3(100, 100, 1);
        //wall2.transform.rotation = Quaternion.Euler(0, 15, 0);
        //wall1.transform.position = new Vector3(0, 0, -9);

        var spacing = 3; 

        var wall1 = gl.CreatePlottedWall
            (
               new Vector3(0, 0, 9),
               new Vector3(100, 100, 1), 
               new Vector3(0, 75, 0),
               Color.white,
               true,
               Color.red,
               spacing
            );
        wall1.tag = "Wall";
        wall1.name = "Wall1";

        var wall2 = gl.CreatePlottedWall
            (
               new Vector3(0, 0, 9),
               new Vector3(100, 100, 1),
               new Vector3(0, 15, 0),
               Color.white,
               true,
               Color.red,
               spacing
            );
        wall2.tag = "Wall";
        wall1.name = "Wall2";
    }

    public void ResetLevel()
    {
        this.player.transform.position = new Vector3(0, 1, 0);
    }
}
