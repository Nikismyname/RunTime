using UnityEngine;

public class SceneTest : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Test Awake");
    }

    private void Start()
    {
        Debug.Log("Test Start");
    }

    private void OnEnable()
    {
        Debug.Log("Test On Enable");
    }
}
