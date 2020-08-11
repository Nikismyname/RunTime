using System.CodeDom;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DrawSavedCubesRow : SpellcraftProcUIElement
{
    public DrawSavedCubesRow(Vector2 tl, Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(tl, baseColor, generator, procUI)
    {
    }

    public override GameObject[] GenerateUI(out Vector2 offsets)
    {
        string[] textNames = CubePersistance.GetAllSavedCubes().Select(z => z.Name).ToArray();

        for (int i = 0; i < textNames.Length; i++)
        {
            string nameText = textNames[i];

            GameObject main =
                this.Generator.DrawButton(nameText, new Vector2(Tl.x, Tl.y - i * (this.ProcUI.buttonPixelsY + this.ProcUI.yOffset)));
            GameObject delete = this.Generator.DrawButton("X",
                new Vector2(Tl.x + this.ProcUI.buttonPixelsX + this.ProcUI.xOffset, Tl.y - i * (this.ProcUI.buttonPixelsY + this.ProcUI.yOffset)),
                new Vector2(30, 30), Color.red);

            main.GetComponent<Button>().onClick.AddListener(() => this.OnClickLoadCube(nameText));
            delete.GetComponent<Button>().onClick.AddListener(() => this.OnClickDeleteCube(nameText));

            this.Elements.Add(main);
            this.Elements.Add(delete);
        }

        offsets = new Vector2(this.ProcUI.buttonPixelsX + this.ProcUI.xOffset * 2 + 30, 0);

        return this.Elements.ToArray();
    }

    public override void Refresh()
    {
        throw new System.NotImplementedException();
    }
    
    private void OnClickDeleteCube(string nameText)
    {
        CubePersistance.DeleteCube(nameText);
        this.Refresh();
    }
    
    private void OnClickLoadCube(string cubeName)
    {
        ActionKeyPersistance.ActionKeyPersistanceData[] mappings = ActionKeyPersistance.GetKeyCubeMapping();

        if (mappings.Any(x => x.CubeName == cubeName))
        {
            Debug.Log("There is already key with that cube name!!!");
            return;
        }

        SpellcraftProcUI.ActionButtonMap selected = this.ProcUI.actionButtonData.SingleOrDefault(x => x.Selected);

        if (selected == null) return;

        selected.SetName(cubeName);
        selected.Deselect();
        ActionKeyPersistance.Persist(new ActionKeyPersistance.ActionKeyPersistanceData
        {
            CubeName = cubeName,
            KeyId = selected.Id,
        });
    }
}