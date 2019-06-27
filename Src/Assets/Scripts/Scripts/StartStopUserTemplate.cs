using UnityEngine;

public class StartStopUserTemplateSource: MonoBehaviour
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

    public static StartStopUserTemplateSource Attach(GameObject obj)
    {
        return obj.AddComponent<StartStopUserTemplateSource>();
    }
}
