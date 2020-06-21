using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to ShowCodeButton
/// </summary>
public class ShowCodeBehaviour : MonoBehaviour
{
    private bool show;
    private GameObject textEditor;
    private TMP_InputField inputField;
    private Button button;
    private string lastText = string.Empty;
    private string lastCurrent = string.Empty;

    void Start()
    {
        this.textEditor = GameObject.Find("TextEditor");
        this.inputField = this.textEditor.GetComponent<TMP_InputField>();
        this.textEditor.SetActive(false);
        this.show = true;
        this.button = gameObject.GetComponent<Button>();
        this.button.onClick.AddListener(OnClick);

        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = Path.GetDirectoryName(app_settings.currentPath);
        watcher.Filter = Path.GetFileName(app_settings.currentPath);
        watcher.EnableRaisingEvents = true;
        watcher.Changed += new FileSystemEventHandler((source, e) =>
        {
            Debug.LogWarning("Updating InGame");
            string text = File.ReadAllText(app_settings.currentPath);
            this.lastCurrent = text;
            this.inputField.text = File.ReadAllText(app_settings.currentPath);
        });
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

    public void UpdateCurrentFileWhenNotEsc()
    {
        if (this.lastCurrent != this.inputField.text)
        {
            Debug.Log("Updating Current");
            File.WriteAllText(app_settings.currentPath, this.inputField.text);
            this.lastCurrent = this.inputField.text;
        }
    }

    public void SetText(string text)
    {
        Debug.Log("SET_TEXT");
        this.inputField.text = text;
        this.UpdateCurrentFileWhenNotEsc();
    }

    public string GetText()
    {
        return this.inputField.text;
    }
}
