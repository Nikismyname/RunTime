using UnityEngine;

public class BasicUserTemplateSource : MonoBehaviour
{
    GameObject g;
    void Start()
    {
        g = gameObject;
    }

    void Update()
    {

    }

    public static BasicUserTemplateSource Attach(GameObject obj)
    {
        return obj.AddComponent<BasicUserTemplateSource>();
    }
}
