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
        sceneManager.SpecificLevel(typeof(WallJumpMain));
    }

    public void Level_1()
    {
        sceneManager.SpecificLevel(typeof(Level1Main));
    }

    public void Level_3()
    {
        sceneManager.SpecificLevel(typeof(Level3Main));
    }

    public static string Source { get; set; } = @"";
}

