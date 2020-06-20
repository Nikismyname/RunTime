using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowCodeBehaviour : MonoBehaviour
{
    private bool show;
    private GameObject textEditor;
    private TMP_InputField inputField;
    private Button button;
    private string lastText = string.Empty;

    void Start()
    {
        this.textEditor = GameObject.Find("TextEditor");
        var text = Templates.BasicUserTemplate();
        this.inputField = this.textEditor.GetComponent<TMP_InputField>();
        this.inputField.text = text;

        this.textEditor.SetActive(false);

        this.show = true;
        this.button = gameObject.GetComponent<Button>();
        this.button.onClick.AddListener(OnClick);
    }
 
    private void OnClick()
    {
        if (show)
        {
            this.Open();
        }
        else
        {
            this.Close();
        }
    }

    public void Close()
    {
        this.show = true;
        this.textEditor.SetActive(false);
    }

    public void Open()
    {
        this.show = false;
        this.textEditor.SetActive(true);

    }

    public void SaveLast()
    {
        this.lastText = this.inputField.text;
    }

    public void ResoreToLast()
    {
        if (this.lastText != string.Empty)
        {
            this.inputField.text = this.lastText;
        }
    }
}
