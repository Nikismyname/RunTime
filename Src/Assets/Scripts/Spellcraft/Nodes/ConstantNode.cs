using UnityEngine;
using UnityEngine.UI;

public class ConstantNode : MonoBehaviour
{
    private InGameUI UI;
    private object value;

    public void Setup(object value, InGameUI UI)
    {
        this.value = value;
        this.UI = UI;

        gameObject.GetComponent<Button>().onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        Debug.Log("HERE " + this.value.ToString());
        this.UI.RegisterConstantClick(this);
    }

    public object GetVal()
    {
        return this.value;
    }
}

