using UnityEngine;
using UnityEngine.UI;

public class UniUIManager : MonoBehaviour
{
    private GameObject TargetInfo;
    private GameObject TargetInfoText;

    private void Start()
    {
        this.TargetInfo = GameObject.Find("TargetInfo");
        this.TargetInfoText = GameObject.Find("TargetInfoText");
        this.TargetTextClose();
    }

    public void SetTargetText(string text)
    {
        this.TargetInfoText.GetComponent<Text>().text = text;
    }

    public void TargetTextOpen()
    {
        this.TargetInfo.SetActive(true);
    }

    public void TargetTextClose()
    {
        this.TargetInfo.SetActive(false);
    }
}

