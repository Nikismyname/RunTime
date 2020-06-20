namespace Assets.Scripts.TestGame.Common.InitialCodes
{
    public static class InitialCodes
    {
        public static string Level1 { get; set; } = @"

using UnityEngine;

public class BasicUserTemplate : MonoBehaviour
{
    GameObject g;

    void Start()
    {
        g = gameObject;
    }

    void Update()
    {
        g.transform.position += new Vector3(0, - Time.deltaTime * 3, 0);
        //g.transform.position += new Vector3(0, Time.deltaTime * 3, 0);
    }
}

";
        public static string Level3 { get; set; } = @"

using System.Threading.Tasks;
using UnityEngine;

public class Level3 : MonoBehaviour
{
    private GameObject g;
    private GameObject toMove;
    private GameObject destination;

    private void Start()
    {
        this.Some();
    }

    private async void Some()
    {
        this.g = gameObject;
        this.toMove = GameObject.Find(""Level3ToTranport567"");
        this.destination = GameObject.Find(""Level3FinalPosition789"");

        g.transform.position = toMove.transform.position;
        await Task.Delay(1000*3);
        g.transform.position = destination.transform.position;
    }
}

";

    }
}
