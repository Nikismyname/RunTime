using UnityEngine;

public class Level1Main: MonoBehaviour, ILevelMain
{
    private LevelManager lm;
    private GameObject t, goal;
    private Camera camera;
    private Plane[] planes;
    private Collider targetCollider;
    private GenerateLevel gl;
    private Main ms;

    private void Start()
    {
        var main = GameObject.Find("Main");
        this.lm = main.GetComponent<LevelManager>(); 
        this.ms = main.GetComponent<Main>();
        var rb = main.GetComponent<ReferenceBuffer>();
        this.gl = new GenerateLevel(ms, rb);

        this.t = gl.GenerateEntity(EntityType.Target, new Vector3(0, -5, 0), PrimitiveType.Cube, Color.gray,null,"Target");
        this.goal = gl.GenerateEntity(EntityType.NonTarget, new Vector3(0, 5, 0), PrimitiveType.Cube, Color.blue,null, "Goal");

        this.camera = Camera.main;
        camera.transform.position = new Vector3(0, 0, -20);
        camera.transform.eulerAngles = new Vector3(0,0,0);

        this.planes = GeometryUtility.CalculateFrustumPlanes(camera);
        this.targetCollider = t.GetComponent<Collider>();
        Debug.Log(targetCollider);
    }

    private void Update()
    {
        if (GeometryUtility.TestPlanesAABB(this.planes, this.targetCollider.bounds))
        {
        }
        else
        {
            lm.Failure("Target out of camera view!");
        }

        if (this.t.transform.position.y > this.goal.transform.position.y)
        {
            this.lm.Success(); 
        }
    }

    public void ResetLevel()
    {
        Destroy(this.t);
        this.ms.UnregeisterTarget(this.t);
        this.t = this.gl.GenerateEntity(EntityType.Target, new Vector3(0, -5, 0), PrimitiveType.Cube, Color.gray, null, "Target");
        this.targetCollider = this.t.GetComponent<Collider>();
    }
}
