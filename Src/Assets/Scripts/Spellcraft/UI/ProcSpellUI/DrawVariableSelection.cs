using UnityEngine;
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
        
        VariableInfo[] variables =
        {
            new VariableInfo(ResultCanvas.PlayerMarkerVarName),
            new VariableInfo(ResultCanvas.PlayerForwardVarName),
            new VariableInfo(ResultCanvas.DroneMarkerVarName),
            new VariableInfo(ResultCanvas.PlayerPositionVarName)
        };

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                int index = yy * 3 + xx;

                if (index >= variables.Length)
                {
                    goto label;
                }

                GameObject variableButton = this.generator.DrawButton(variables[index].Name,
                    new Vector2(tl.x + this.procUI.xOffset + xx * (this.procUI.xOffset + this.procUI.buttonPixelsX),
                        tl.y - (yy * (this.procUI.yOffset + this.procUI.buttonPixelsY))));
                this.Elements.Add(variableButton);

                variableButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                });
                
                this.selectableButtons.RegisterButton(variableButton.GetComponent<Button>(), isSelected =>
                { 
                    VariableInfo variable = variables[index];
                        
                    if (isSelected)
                    {
                        if (variable.Id == -1)
                        {
                            Debug.LogError("ID NOT ASSIGNED!!!");
                        }
                        
                        ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.UnregisterDirectInput(variable.Id);
                        return false;
                    }
                    else
                    {
                        int id = ReferenceBuffer.Instance.worldSpaceUI.dynamicSetup.AddVariable(variable.Name);
                        variable.Id = id;
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

    public class VariableInfo
    {
        public VariableInfo(string name)
        {
            this.Name = name;
            this.Id = -1;
        }

        public string Name { get; set; }
        public int Id { get; set; }
    }
}