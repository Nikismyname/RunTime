using UnityEngine;

public class NextLevelSphere : MonoBehaviour
{
    private int clicked;
    private MySceneManager sceneManager;

    private void Start()
    {
        this.clicked = 0;
        this.sceneManager = GameObject.Find("SceneManager")?.GetComponent<MySceneManager>();
    }

    private void OnMouseDown()
    {
        this.clicked++; 

        if(this.clicked == 1)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.blue; 
        }

        if(this.clicked == 2)
        {
            if(this.sceneManager == null)
            {
                Debug.Log("Scene Manager Not Found!");
            }
            else
            {
                this.sceneManager.NextLevel(); 
            }
        }
    }
}
