using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DrawSavedCubesRow : SpellcraftProcUIElement
{
    public DrawSavedCubesRow(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(baseColor, generator, procUI)
    {
    }

    protected override GameObject[] GenerateUI(out Vector2 offsets)
    {
        string[] textNames = CubePersistance.GetAllSavedCubes().Select(z => z.Name).ToArray();

        for (int i = 0; i < textNames.Length; i++)
        {
            string nameText = textNames[i];

            float mainX = tl.x;
            float mainY = tl.y - i * (this.procUI.buttonPixelsY + this.procUI.yOffset);

            if(i == 0)
                Debug.Log($"{mainX} {mainY}");

            float delX = tl.x + this.procUI.buttonPixelsX + this.procUI.xOffset;
            float delY = tl.y - i * (this.procUI.buttonPixelsY + this.procUI.yOffset); 
            
            GameObject main = this.generator.DrawButton(nameText, new Vector2(mainX, mainY));
            GameObject delete = this.generator.DrawButton("X", new Vector2(delX, delY), new Vector2(30, 30), Color.red);

            main.GetComponent<Button>().onClick.AddListener(() => this.OnClickLoadCube(nameText));
            delete.GetComponent<Button>().onClick.AddListener(() => this.OnClickDeleteCube(nameText));

            this.Elements.Add(main);
            this.Elements.Add(delete);
        }

        offsets = new Vector2(this.procUI.buttonPixelsX + this.procUI.xOffset * 2 + 30, 0);

        return this.Elements.ToArray();
    }

    private void OnClickDeleteCube(string nameText)
    {
        CubePersistance.DeleteCube(nameText);
        ActionKeyPersistance.Delete(nameText);
        this.procUI.drawActionButtonMapping.Refresh();
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

        DrawActionButtonMapping.ActionButtonMap selected = this.procUI.drawActionButtonMapping.actionButtonData.SingleOrDefault(x => x.Selected);

        if (selected == null) return;

        selected.SetName(cubeName);
        selected.Deselect();
        ActionKeyPersistance.Persist(new ActionKeyPersistance.ActionKeyPersistanceData
        {
            CubeName = cubeName,
            KeyId = selected.Id,
        });

        ReferenceBuffer.Instance.ShowPresetBehaviour.RedoMappingConnections();
    }
}