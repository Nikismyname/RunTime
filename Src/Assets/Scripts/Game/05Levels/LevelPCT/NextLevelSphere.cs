using UnityEngine;
using UnityEngine.EventSystems;

public class NextLevelSphere : MonoBehaviour, IPointerDownHandler
{
    private int clicked;
    private MySceneManager sceneManager;

    private void Start()
    {
        this.clicked = 0;
        this.sceneManager = GameObject.Find("SceneManager")?.GetComponent<MySceneManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.clicked++;

        if (this.clicked == 1)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }

        if (this.clicked == 2)
        {
            if (this.sceneManager == null)
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
