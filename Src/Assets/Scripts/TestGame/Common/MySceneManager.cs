using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    private int level = 1;
    private bool midLevel = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("LevelPCT");
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Level" + this.level);
        this.level++;
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
        this.level = level + 1;
    }

    public void WallJump()
    {
        SceneManager.LoadScene("WallJump"); 
    }

    public void Home()
    {
        SceneManager.LoadScene("LevelPCT");
    }
}
