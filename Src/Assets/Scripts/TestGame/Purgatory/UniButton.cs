//using UnityEngine;
//using UnityEngine.UI;

//public class UniButton: MonoBehaviour
//{
//    public string name;
//    private MySceneManager sceneManager;

//    private void Start()
//    {
//        this.sceneManager = GameObject.Find("SceneManager")?.GetComponent<MySceneManager>();
//        this.gameObject.GetComponent<Button>().onClick.AddListener(this.OnClick);
//    }

//    public void OnClick()
//    {
//        this.NextLevel();
//        this.SameLevel();
//        this.PrevLevel();
//        this.Home();
//    }

//    public void NextLevel()
//    {
//        if (name == "next")
//        {
//            this.sceneManager.NextLevel();
//        }
//    }

//    public void SameLevel()
//    {
//        if (name == "same")
//        {
//            this.sceneManager.SameLevel();
//        }
//    }

//    public void PrevLevel()
//    {
//        if(name == "prev")
//        {
//            this.sceneManager.PrevLevel();
//        }
//    }

//    public void Home()
//    {
//        if (name == "home")
//        {
//            this.sceneManager.Home();
//        }
//    }
//}

