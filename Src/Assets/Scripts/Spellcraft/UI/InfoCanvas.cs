using TMPro;
using UnityEngine;

public class InfoCanvas
{
    private GameObject textPrefab;
    private GameObject textCanvas;
    private TMP_Text text;
    private RectTransform rectTransform;
    private Camera camera; 

    public InfoCanvas(GameObject textPrefab, Camera camera, Transform parent)
    {
        this.textPrefab = textPrefab;
        this.textCanvas = GameObject.Instantiate(this.textPrefab);
        this.textCanvas.transform.SetParent(parent);
        this.text = this.textCanvas.transform.Find("Text").GetComponent<TMP_Text>();
        this.rectTransform = textCanvas.GetComponent<RectTransform>();
        this.camera = camera;
    }

    public void SetTextWorldCanvasText(string text)
    {
        this.text.text = text;
    }
    
    public void SetTextWorldCanvasPosition(Vector3 position)
    {
        this.rectTransform.position = position;
        this.rectTransform.LookAt(this.rectTransform.transform.position + this.camera.transform.forward);
    }

    public void Reset()
    {
        this.SetTextWorldCanvasText(string.Empty);
        this.SetTextWorldCanvasPosition(new Vector3(1000, 1000, 1000));
    }
}

