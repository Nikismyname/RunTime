using Boo.Lang;
using UnityEngine;
using UnityEngine.UI;

public class TurretLevel : MonoBehaviour, ILevelMain
{
    public List<GameObject> targets = new List<GameObject>();
    private List<GenericBoolet> boolets = new List<GenericBoolet>();
    private GameObject turret;
    private TurretInt tt;
    private float booletSpeed = 4f;

    private void Start()
    {
        var rb = ReferenceBuffer.Instance;
        rb.gl.CylinderBasePrefabStand1();
        var player = rb.gl.PlayerWithCamStand1();
        rb.InfoTextObject.GetComponent<Text>().text = ProblemDesctiptions.Tutorial1StartMethod2;
        rb.ShowCode.SetText(InitialCodes.Tutorial1StartMethod2);

        targets.Add(
        ReferenceBuffer.Instance.gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(10, 10, 10),
            PrimitiveType.Sphere,
            Color.white,
            null, "TurretTarget1"));

        targets.Add(
        ReferenceBuffer.Instance.gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(-10, 10, 10),
            PrimitiveType.Sphere,
            Color.white,
            null, "TurretTarget2"));

        targets.Add(
        ReferenceBuffer.Instance.gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(10, 10, -10),
            PrimitiveType.Sphere,
            Color.white,
            null, "TurretTarget3"));

        this.turret = ReferenceBuffer.Instance.gl.GenerateEntity(
           EntityType.Target,
           new Vector3(0, 0, 0),
           PrimitiveType.Capsule,
           Color.white,
           null, "turret");

        this.tt = ReferenceBuffer.Instance.gl.AddEditableScriptToEntity<TurretInt>(this.turret, TurretInt.Source);
        this.tt.Setup(this);

        this.tt.ShootLoop();
    }

    private void Update()
    {
        List<GenericBoolet> toRemove = new List<GenericBoolet>(); 
        
        foreach (var boolet in this.boolets)
        {
            boolet.Body.transform.position += boolet.Velocity.normalized * Time.deltaTime * this.booletSpeed; 

            if((boolet.Body.transform.position - boolet.InitialPosition).magnitude > 30)
            {
                toRemove.Add(boolet);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            var boolet = toRemove[i];
            Destroy(boolet.Body);

            this.boolets.Remove(boolet);
        }
    }

    public void ShootBoolet(GenericBoolet boolet)
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        body.SetCollor(Color.red);
        body.transform.position = boolet.InitialPosition;
        boolet.Body = body;

        this.boolets.Add(boolet);
    }

    public void ResetLevel()
    {
        ReferenceBuffer.Instance.MySceneManager.SameLevel();
    }
}

