﻿using UnityEngine;
using UnityEngine.UI;

public class DrawVariableSelection : SpellcraftProcUIElement
{
    private SelectableButtons selectableButtons = new SelectableButtons();

    public DrawVariableSelection(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(baseColor, generator, procUI)
    {
    }

    protected override GameObject[] GenerateUI(out Vector2 offsets)
    {
        this.tl = tl;
        
        string[] variableNames =
        {
            ResultCanvas.PlayerMarkerVarName, ResultCanvas.PlayerForwardVarName, ResultCanvas.DroneMarkerVarName,
            ResultCanvas.PlayerPositionVarName
        };

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                int index = yy * 3 + xx;

                if (index >= variableNames.Length)
                {
                    goto label;
                }

                GameObject variableButton = this.generator.DrawButton(variableNames[index],
                    new Vector2(tl.x + this.procUI.xOffset + xx * (this.procUI.xOffset + this.procUI.buttonPixelsX),
                        tl.y - (yy * (this.procUI.yOffset + this.procUI.buttonPixelsY))));
                this.Elements.Add(variableButton);

                variableButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                });
                
                this.selectableButtons.RegisterButton(variableButton.GetComponent<Button>(), isSelected =>
                {
                    if (isSelected)
                    {
                        return false;
                    }
                    else
                    {
                        ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.AddVariable(variableNames[index]);
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