using UnityEngine;

public class TeleportationBehaviour: MonoBehaviour
{
    private MySceneManager sceneManager;

    private void Start()
    {
        this.sceneManager = GameObject.Find("SceneManager")?.GetComponent<MySceneManager>();
    }

    public void Home()
    {
        sceneManager.Home(); 
    }

    public void WallJump()
    {
        sceneManager.WallJump();
    }

    public void Level_1()
    {
        sceneManager.LoadLevel(1);
    }

    public void Level_2()
    {
        sceneManager.LoadLevel(2);
    }

    public void Level_3()
    {
        sceneManager.LoadLevel(3);
    }
}
