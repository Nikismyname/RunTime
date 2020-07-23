using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public Type[] levels = new Type[] {typeof(LevelMainRPG) ,typeof(LevelPCTMain), typeof (Tutiral3GOManip2), typeof(Tutiral3GOManip1), typeof(TurretLevel), typeof(Tutiral1StartMethod2), typeof(Tutiral1StartMethod1), typeof(Level1Main), typeof(Level3Main), typeof(WallJumpMain) };
    private int level = 0;

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
            LevelBase levelMain;
            if (type == null)
            {
                levelMain = (LevelBase)main.AddComponent(this.GetCurrentLevelType());
            }
            else
            {
                levelMain = (LevelBase)main.AddComponent(type);
            }

            ReferenceBuffer.Instance.Level = levelMain;

            levelManager.levelMono = (MonoBehaviour)levelMain;
            levelManager.Setup();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.ToString());
        }
    }
}
