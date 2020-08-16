using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DrawTypesSelection: SpellcraftProcUIElement
{
    private SelectableButtons selectableButtons = new SelectableButtons();
    
    //DiContainer container = new DiContainer();

    public DrawTypesSelection(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(
        baseColor, generator, procUI)
    {
        //this.selectableButtons = this.container.Instantiate<SelectableButtons>();
    }

    protected override GameObject[] GenerateUI(out Vector2 offsets)
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

                GameObject typeButton = this.generator.DrawButton(typeNames[index],
                    new Vector2(tl.x + this.procUI.xOffset + xx * (this.procUI.xOffset + this.procUI.buttonPixelsX),
                        tl.y - (yy * (this.procUI.yOffset + this.procUI.buttonPixelsY))));
                this.Elements.Add(typeButton);

                this.selectableButtons.RegisterButton(typeButton.GetComponent<Button>(), isSelected =>
                {
                    if (isSelected)
                    {
                        Debug.Log("No Deselect Yet!");
                        return false;
                    }
                    else
                    {
                        Debug.Log("TypeSelection" + index);
                        ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.RegisterNode(DynamicSetup.Types[index]);
                        return true;
                    }
                });
            }
        }

        label:

        offsets = new Vector2((this.procUI.xOffset + this.procUI.buttonPixelsX) * 3, (this.procUI.yOffset + this.procUI.buttonPixelsY) * 3 );

        return this.Elements.ToArray();
    }

    public override void Refresh()
    {
        throw new System.NotImplementedException();
    }
}