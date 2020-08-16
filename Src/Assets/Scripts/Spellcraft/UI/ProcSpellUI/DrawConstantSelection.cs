using UnityEngine;
using UnityEngine.UI;

public class DrawConstantSelection : SpellcraftProcUIElement
{
    private SelectableButtons selectableButtons = new SelectableButtons();

    public DrawConstantSelection(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(
        baseColor, generator, procUI)
    {
    }

    protected override GameObject[] GenerateUI(out Vector2 offsets)
    {
        this.tl = tl;

        object[] constants =
        {
            0.5, 
            Vector3.up, 
            2, 
            10,
            1000,
            Vector3.left,
            -1,
        };

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                int index = yy * 3 + xx;

                if (index >= constants.Length)
                {
                    goto label;
                }

                GameObject variableButton = this.generator.DrawButton(constants[index].ToString(),
                    new Vector2(tl.x + this.procUI.xOffset + xx * (this.procUI.xOffset + this.procUI.buttonPixelsX),
                        tl.y - (yy * (this.procUI.yOffset + this.procUI.buttonPixelsY))));
                this.Elements.Add(variableButton);

                variableButton.GetComponent<Button>().onClick.AddListener(() => { });

                this.selectableButtons.RegisterButton(variableButton.GetComponent<Button>(), isSelected =>
                {
                    if (isSelected)
                    {
                        return false;
                    }
                    else
                    {
                        ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.AddConstant(constants[index]);
                        return true;
                    }
                });
            }
        }

        label:

        offsets = new Vector2((this.procUI.xOffset + this.procUI.buttonPixelsX) * 3,
            (this.procUI.yOffset + this.procUI.buttonPixelsY) * 3);

        return this.Elements.ToArray();
    }

    public override void Refresh()
    {
        throw new System.NotImplementedException();
    }
}