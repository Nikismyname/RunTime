using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawSavingCube : SpellcraftProcUIElement
{
    public DrawSavingCube(Vector2 tl, Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(
        tl, baseColor, generator, procUI)
    {
    }

    public override GameObject[] GenerateUI(out Vector2 offsets)
    {
        GameObject text = this.Generator.DrawText(new Vector2(Tl.x, Tl.y), "Name The Save", 20);
        GameObject input =
            this.Generator.DrawInputMenu(new Vector2(Tl.x, Tl.y - this.ProcUI.yOffset - this.ProcUI.buttonPixelsY));
        GameObject saveButton = this.Generator.DrawButton("Save",
            new Vector2(Tl.x, Tl.y - (this.ProcUI.yOffset + this.ProcUI.buttonPixelsY) * 2));
        saveButton.GetComponent<Button>().onClick
            .AddListener(() => this.OnClickSave(input.GetComponent<TMP_InputField>()));
        this.Elements.Add(text);
        this.Elements.Add(input);
        this.Elements.Add(saveButton);

        offsets = new Vector2(this.ProcUI.buttonPixelsX + this.ProcUI.yOffset, 0);
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

        this.ProcUI.tracker.Persist(nameText);

        this.Refresh();
    }
}