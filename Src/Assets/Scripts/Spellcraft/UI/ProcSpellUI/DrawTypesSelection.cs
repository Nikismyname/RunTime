using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DrawTypesSelection: SpellcraftProcUIElement
{
    public DrawTypesSelection(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(baseColor, generator, procUI)
    {
    }

    public override GameObject[] GenerateUI(Vector2 tl, out Vector2 offsets)
    {
        this.tl = tl; 
        
        string[] typeNames = DynamicSetup.Types.Select(z => z.Name).ToArray();

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                int index = yy * 3 + xx;

                if (index >= typeNames.Length)
                {
                    goto label;
                }

                GameObject variableButton = this.generator.DrawButton(typeNames[index],
                    new Vector2(tl.x + this.procUI.xOffset + xx * (this.procUI.xOffset + this.procUI.buttonPixelsX),
                        tl.y - (yy * (this.procUI.yOffset + this.procUI.buttonPixelsY))));
                this.Elements.Add(variableButton);

                variableButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.RegisterNode<int>(DynamicSetup.Types[index]);
                });
            }
        }

        label:

        offsets = new Vector2((this.procUI.xOffset + this.procUI.buttonPixelsX) * 3, 0);
        return this.Elements.ToArray(); 
    }

    public override void Refresh()
    {
        throw new System.NotImplementedException();
    }
}