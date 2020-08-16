using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawActionButtonMapping : SpellcraftProcUIElement
{
    public List<ActionButtonMap> actionButtonData = new List<ActionButtonMap>();

    public DrawActionButtonMapping(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI) : base(baseColor, generator, procUI)
    {
    }

    protected override GameObject[] GenerateUI(out Vector2 offsets)
    {
        this.tl = tl; 
        
        ActionKeyPersistance.ActionKeyPersistanceData[] mappings = ActionKeyPersistance.GetKeyCubeMapping();

        for (int yy = 0; yy < 3; yy++)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                GameObject mapButton = this.generator.DrawButton("",
                    new Vector2(tl.x + this.procUI.xOffset + xx * (this.procUI.xOffset + this.procUI.buttonPixelsX),
                        tl.y - (yy * (this.procUI.yOffset + this.procUI.buttonPixelsY))));
                this.Elements.Add(mapButton);
                int keyId = yy * 3 + xx + 1;
                ActionButtonMap node = new ActionButtonMap(keyId, mapButton, this.actionButtonData);
                ActionKeyPersistance.ActionKeyPersistanceData mapping = mappings.FirstOrDefault(y => y.KeyId == keyId);

                if (mapping != null)
                {
                    node.SetName(mapping.CubeName);
                }

                this.actionButtonData.Add(node);
                mapButton.GetComponent<Button>().onClick.AddListener(() => node.Select());
            }
        }

        offsets = new Vector2((this.procUI.xOffset + this.procUI.buttonPixelsX) * 3, (this.procUI.yOffset + this.procUI.buttonPixelsY) * 3 );
        
        return this.Elements.ToArray(); 
    }

    public class ActionButtonMap
    {
        public ActionButtonMap(int id, GameObject gameObject, List<ActionButtonMap> all)
        {
            this.Id = id;
            this.GameObject = gameObject;
            this.all = all;
        }

        private List<ActionButtonMap> all;

        public int Id { get; }

        public GameObject GameObject { get; }

        public string Name { get; set; }

        public bool Selected { get; set; } = false;


        public void SetName(string name)
        {
            this.Name = name;

            this.GameObject.transform.Find("Text").GetComponent<TMP_Text>().text = name;
        }

        public void Select()
        {
            foreach (var item in this.all)
            {
                item.Deselect();
            }

            this.Selected = true;
            this.GameObject.GetComponent<Image>().color = Color.green;
        }

        public void Deselect()
        {
            this.Selected = false;
            this.GameObject.GetComponent<Image>().color = Color.white;
        }
    }
}