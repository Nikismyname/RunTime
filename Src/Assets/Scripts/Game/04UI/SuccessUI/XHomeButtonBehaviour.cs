using UnityEngine;
using UnityEngine.UI;

public class HomeButtonBehaviour : MonoBehaviour
{
    MySceneManager sceneManaget;

    void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);
        this.sceneManaget = GameObject.Find("SceneManager")?.GetComponent<MySceneManager>();
    }

    private void OnClick()
    {
        this.sceneManaget.Home();
    }
}
