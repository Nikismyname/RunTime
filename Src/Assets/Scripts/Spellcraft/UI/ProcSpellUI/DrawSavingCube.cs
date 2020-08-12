using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawSavingCube : SpellcraftProcUIElement
{
    public DrawSavingCube(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(baseColor, generator, procUI)
    {
    }

    public override GameObject[] GenerateUI(Vector2 tl, out Vector2 offsets)
    {
        this.tl = tl;
        
        GameObject text = this.generator.DrawText(new Vector2(tl.x, tl.y), "Name The Save", 20);
        GameObject input =
            this.generator.DrawInputMenu(new Vector2(tl.x, tl.y - this.procUI.yOffset - this.procUI.buttonPixelsY));
        GameObject saveButton = this.generator.DrawButton("Save",
            new Vector2(tl.x, tl.y - (this.procUI.yOffset + this.procUI.buttonPixelsY) * 2));
        saveButton.GetComponent<Button>().onClick
            .AddListener(() => this.OnClickSave(input.GetComponent<TMP_InputField>()));
        this.Elements.Add(text);
        this.Elements.Add(input);
        this.Elements.Add(saveButton);

        offsets = new Vector2(this.procUI.buttonPixelsX + this.procUI.yOffset, 0);

        return this.Elements.ToArray();
    }

    public override void Refresh()
    {
        throw new System.NotImplementedException();
    }

    private void OnClickSave(TMP_InputField nameInput)
    {
        string nameText = nameInput.text;

        string[] existingNames = CubePersistance.GetAllSavedCubes().Select(z => z.Name).ToArray();

        if (string.IsNullOrWhiteSpace(nameText) || nameText.Length < 5 || existingNames.Contains(nameText))
        {
            Debug.Log("Invalid Name!");
            return;
        }

        Debug.Log("VALID Name!");

        this.procUI.tracker.Persist(nameText);

        this.Refresh();
    }
}