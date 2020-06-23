public static class InitialCodes
{
    public static string RestrictionMarker = "///Only Add Code Here";

    public static string Blank { get; set; } = @"

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

    }
}

";

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

    public static string Tutorial1StartMethod1 { get; set; } = @"

using System.Threading.Tasks;
using UnityEngine;

public class Tutorial1StartMenu1 : MonoBehaviour
{
    private void Start()
    {
        ///Only Add Code Here>
        Move(new Vector3(0,10,0));
        ///Only Add Code Here^
    }

    public async void Move(Vector3 offset)
    {
        Vector3 initPos = gameObject.transform.position;
        Vector3 finalPos = initPos + offset; 

        for (int i = 0; i <= 100; i++)
        {
            gameObject.transform.position = Vector3.Lerp(initPos, finalPos, (float)i / 100f);
            await Task.Delay(30);
        }
    }
}

";

    public static string Tutorial1StartMethod2 { get; set; } = @"

using System.Threading.Tasks;
using UnityEngine;

public class Tutorial1StartMenu2 : MonoBehaviour
{
    private async void Start()
    {
        ///Only Add Code Here>
        await Move(new Vector3(0, 4, 0));
        await Move(new Vector3(0, -8, 0));
        await Move(new Vector3(0, 4, 0));

        await Move(new Vector3(4, 0, 0));
        await Move(new Vector3(-8, 0, 0));
        await Move(new Vector3(4, 0, 0));

        await Move(new Vector3(0, 0, 4));
        await Move(new Vector3(0, 0, -8));
        ///Only Add Code Here^
    }

    public async Task Move(Vector3 offset)
    {
        Vector3 initPos = gameObject.transform.position;
        Vector3 finalPos = initPos + offset;

        for (int i = 0; i <= 100; i++)
        {
            gameObject.transform.position = Vector3.Lerp(initPos, finalPos, (float)i / 100f);
            await Task.Delay(10);
        }
    }
}

";
}