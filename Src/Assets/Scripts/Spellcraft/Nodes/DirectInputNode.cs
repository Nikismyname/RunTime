using UnityEngine;
using UnityEngine.UI;

public class DirectInputNode : MonoBehaviour
{
    public InputCanvas.InputElements elements;
    public ParameterNode paramNode = null;
    public string VariableName;
    private WorldSpaceUI UI;
    private object value;
    private Color originalColor;
    private Color selectedColor = Color.gray;
    private Color paramSelectedColor = Color.green;
    private bool used = false;
    private bool isVariable;

    public int ID { get; set; }

    public void Setup(object value, int ID, WorldSpaceUI UI, InputCanvas.InputElements elements, bool isVariable = false, string variableName = "constant")
    {
        this.value = value;
        this.UI = UI;
        this.elements = elements;
        this.isVariable = isVariable;
        this.VariableName = variableName;

        ///Reusing the Constant code for variables too, will fix semantics later
        if (this.isVariable)
        {
            this.originalColor = Color.Lerp(Color.red, Color.yellow, 0.5f).SetAlpha();
            this.elements.Button.GetComponent<Image>().color = this.originalColor;
        }
        else
        {
            this.originalColor = this.elements.Button.GetComponent<Image>().color;
        }

        gameObject.GetComponent<Button>().onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        //Debug.Log("HERE " + this.value.ToString());
        this.UI.connRegisterer.RegisterConstantClick(this);
    }

    public object GetVal()
    {
        return this.value;
    }

    public void SetUsed(bool used, ParameterNode node)
    {
        this.used = used;
        this.paramNode = node;
        this.elements.Used = used;
    }

    public void FixColorBeforeShow(ParameterNode node)
    {
        if (this.used)
        {
            if (this.paramNode == node)
            {
                this.SetColor(this.paramSelectedColor);
            }
            else
            {
                this.SetColor(this.selectedColor);
            }
        }
        else
        {
            this.SetColor(this.originalColor);
        }
    }

    private void SetColor(Color color)
    {
        this.elements.Button.GetComponent<Image>().color = color;
    }

    public bool IsVariable()
    {
        return this.isVariable;
    }
}

