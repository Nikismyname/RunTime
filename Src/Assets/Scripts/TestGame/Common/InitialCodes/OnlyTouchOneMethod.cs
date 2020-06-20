using UnityEngine.Subsystems;

public static class OnlyTouchOneMethod
{
    public static string OneTouchLevel1 = @"

using UnityEngine;

public class OneTouchLevel1 : MonoBehaviour
{
    GameObject g;
    void Start()
    {
        g = gameObject;
    }

    private void DoSome1() {
        
    }
}

";
}
