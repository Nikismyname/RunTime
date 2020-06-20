using Assets.Scripts.TestGame.Common.InitialCodes;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Level3Main : MonoBehaviour, ILevelMain
{
    Main ms;

    private bool gatheredTheSphere;
    private bool placedTHeSphere;
    private LevelManager lm;
    private GameObject target;
    private GameObject toTransport;
    private GameObject destination;
    private GameObject player;
    private GameObject mainCamera;
    private Vector3 initialPlayerPosition = new Vector3(0, 0, 10);
    private float checkDist = 1;

    void Start()
    {
        /// References!
        var main = GameObject.Find("Main");
        this.lm = main.GetComponent<LevelManager>();
        var rb = main.GetComponent<ReferenceBuffer>();
        this.ms = main.GetComponent<Main>();
        var gl = new GenerateLevel(this.ms, rb);
        ///

        /// Problem Text and Code!
        var infoText = rb.InfoTextObject;
        infoText.GetComponent<Text>().text = ProblemDesctiptions.level3;
        rb.TextEditorInputField.text = InitialCodes.Level3;
        ///...

        /// Value Initialization
        this.gatheredTheSphere = false;
        this.placedTHeSphere = false;
        ///...

        /// Base
        GameObject baseCylinder = gl.CylinderBasePrefab(new Vector3(30, 1, 30), true);
        ///...

        /// Player and cam!
        this.player = gl.Player(this.initialPlayerPosition, true, true, true);
        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;
        ///...

        /// Enitites!
        this.target = gl.GenerateEntity(
            EntityType.Target,
            new Vector3(7, 0, 0),
            PrimitiveType.Sphere,
            Color.white,
            null, "Level3Actor123");

        this.toTransport = gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(-7, 0, 0),
            PrimitiveType.Sphere,
            Color.red,
            null, "Level3ToTranport567");

        this.destination = gl.GenerateEntity(
            EntityType.NonTarget,
            new Vector3(5, 0, 5),
            PrimitiveType.Cube,
            Color.gray,
            null, "Level3FinalPosition789");
        ///...
    }

    private void Update()
    {
        this.UpdateTick();
    }

    private async void UpdateTick()
    {
        if (this.gatheredTheSphere == false)
        {
            if (this.AreClose(this.toTransport))
            {
                await Task.Delay(1000 * 2);
                if (this.AreClose(this.toTransport))
                {
                    this.gatheredTheSphere = true;
                    this.toTransport.SetActive(false);
                    this.target.SetCollor(Color.cyan);
                }
            }
        }
        else if (this.placedTHeSphere == false)
        {
            if (this.AreClose(this.destination))
            {
                this.placedTHeSphere = true;
                Destroy(this.destination);
                this.lm.Success();
            }
        }
    }

    private bool AreClose(GameObject other)
    {
        if ((this.target.transform.position - other.transform.position).magnitude <= this.checkDist)
        {
            return true;
        }

        return false;
    }

    public void ResetLevel()
    {
        this.player.transform.position = this.initialPlayerPosition;
    }
}
