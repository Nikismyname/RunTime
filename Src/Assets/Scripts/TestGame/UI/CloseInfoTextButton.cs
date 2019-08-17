using UnityEngine;
using UnityEngine.UI;

public class CloseInfoTextButton : MonoBehaviour
{
    GameObject parent; 
    void Start()
    {
        parent = gameObject.transform.parent.parent.parent.gameObject;
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick); 
    }

    void OnClick()
    {
        parent.SetActive(false);
    }
}
