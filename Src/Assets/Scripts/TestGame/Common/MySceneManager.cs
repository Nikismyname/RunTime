using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    private int level = 0;
    private Type[] levels = new Type[] { typeof(LevelPCTMain), typeof(Tutiral1StartMethod2), typeof(Tutiral1StartMethod1), typeof(Level1Main), typeof(Level3Main), typeof(WallJumpMain) };

    private readonly string levelScene = "UniLevel";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        this.SameLevel();
    }

    public void SpecificLevel(Type type)
    {
        this.GoToLevelsScene(this.levelScene, type);
    }

    public void NextLevel()
    {
        int levelCount = this.levels.Length;

        if (this.level < levelCount - 1)
        {
            this.level++;
            this.GoToLevelsScene(this.levelScene);
        }
        else
        {
            Debug.LogWarning("You are on max level already!");
        }
    }

    public void PrevLevel()
    {
        int levelCount = this.levels.Length;

        if (this.level > 0)
        {
            this.level--;
            this.GoToLevelsScene(this.levelScene);
        }
        else
        {
            Debug.LogWarning("You are on min level already!");
        }
    }

    public void SameLevel()
    {
        this.GoToLevelsScene(this.levelScene);
    }

    public void SpecificLevel(int newLevel)
    {
        int levelCount = this.levels.Length;

        if (newLevel >= 0 && newLevel < levelCount)
        {
            this.level = newLevel;
            this.GoToLevelsScene(this.levelScene);
        }
        else
        {
            Debug.LogWarning("Passed level is out of range!");
        }
    }

    public Type GetCurrentLevelType()
    {
        return this.levels[this.level];
    }

    public void Home()
    {
        this.GoToLevelsScene(this.levelScene, typeof(LevelPCTMain));
    }

    private void GoToLevelsScene(string sceneName, Type levelType = null)
    {
        StartCoroutine(LoadYourAsyncScene(sceneName, levelType));
    }

    IEnumerator LoadYourAsyncScene(string scene, Type levelType)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (levelType == null)
        {
            this.SetUpScene();
        }
        else
        {
            this.SetUpScene(levelType);
        }
    }

    private void SetUpScene(Type type = null)
    {
        try
        {
            GameObject main = GameObject.Find("Main");
            LevelManager levelManager = main.GetComponent<LevelManager>();
            ILevelMain levelMain;
            if (type == null)
            {
                levelMain = (ILevelMain)main.AddComponent(this.GetCurrentLevelType());
            }
            else
            {
                levelMain = (ILevelMain)main.AddComponent(type);
            }

            levelManager.levelMono = (MonoBehaviour)levelMain;
            levelManager.Setup();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.ToString());
        }
    }
}
