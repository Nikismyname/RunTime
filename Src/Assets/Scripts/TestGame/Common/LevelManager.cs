using UnityEngine;

public class LevelManager : MonoBehaviour 
{
    public MonoBehaviour levelMono;
    private ILevelMain level;
    private GameObject successCanvas;

    public void Setup()
    {
        if (!typeof(ILevelMain).IsAssignableFrom(levelMono.GetType()))
        {
            Debug.LogError("Pased level does not implement the ILevelMain Interface!");
            Debug.Break();
        }

        this.successCanvas = GameObject.Find("SuccessCanvas");
        this.successCanvas.SetActive(false);

        this.level = (ILevelMain)levelMono;
    }

    public void Failure(string reason)
    {
        Debug.Log("Failed: " + reason);
        this.level.ResetLevel();
    }

    public void Success()
    {
        //this.level.ResetLevel();
        this.successCanvas.SetActive(true);
    }
}
