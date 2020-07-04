using UnityEngine;
using UnityEngine.UI;

public class ShowPresetsBehaviour : MonoBehaviour
{
    private bool show;
    GameObject[] buttons; 

    void Start()
    {
        this.show = true;
        this.buttons = new GameObject[9]; 

        for (int i = 1; i <= 9; i++)
        {
            buttons[i - 1] = GameObject.Find("ActionButton" + i);
            buttons[i - 1].SetActive(false); 
        }

        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (this.show)
        {
            this.show = false;
            foreach (var button in this.buttons)
            {
                button.SetActive(true);
            }
        }
        else
        {
            this.show = true;
            foreach (var button in this.buttons)
            {
                button.SetActive(false);
            }
        }
    }
}
