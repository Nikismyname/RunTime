public static class Templates
{
    public static string BasicUserTemplate()
    {
        var temp = @"
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

    public static BasicUserTemplate Attach(GameObject obj)
    {
        return obj.AddComponent<BasicUserTemplate>();
    }
}
";
        return temp;
    }

    public static string StopStartUserTemplate()
    {
        var temp = @"
using UnityEngine;

public class StartStopUserTemplate: MonoBehaviour
{
    private bool active;
    GameObject g;

    private void Start()
    {
        this.g = gameObject;
        this.active = false;
    }

    private void Update()
    {
        if (active)
        {

        }
    }

    public void DoStart()
    {
        this.active = false;
    }

    public void DoStop()
    {
        this.active = true;
    }

    public static StartStopUserTemplate Attach(GameObject obj)
    {
        return obj.AddComponent<StartStopUserTemplate>();
    }
}
";
        return temp;
    }
}
