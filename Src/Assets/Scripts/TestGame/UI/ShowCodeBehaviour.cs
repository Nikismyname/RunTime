using UnityEngine;
using UnityEngine.UI;

public class ShowCodeBehaviour : MonoBehaviour
{
    private bool show;
    private GameObject textEditor;

    void Start()
    {
        this.textEditor = GameObject.Find("TextEditor");
        var text = Templates.BasicUserTemplate();
        //this.textEditor.GetComponent<InputField>().multiLine=true;
        //this.textEditor.GetComponent<InputField>().SetTextWithoutNotify("1\n2");
        //this.textEditor.GetComponent<InputField>().input;
        var inputField = this.textEditor.GetComponent<InputField>();
        inputField.text = text;

        this.textEditor.SetActive(false);

        this.show = true;
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (show)
        {
            this.show = false;
            this.textEditor.SetActive(true); 
        }
        else
        {
            this.show = true;
            this.textEditor.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
