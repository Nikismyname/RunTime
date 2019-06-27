using UnityEngine;
using UnityEngine.UI;

public class ShowActionsBehaviour : MonoBehaviour
{
    private GameObject actions;
    private bool shouldShow = true;
    void Start()
    {
        this.actions = GameObject.Find("ScrollableActions");
        actions.SetActive(false);

        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (this.shouldShow)
        {
            this.actions.SetActive(true);
            this.shouldShow = false;
        }
        else
        {
            this.actions.SetActive(false);
            this.shouldShow = true;
        }
    }
}
