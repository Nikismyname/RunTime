using System.Collections.Generic;
using UnityEngine;

public abstract class SpellcraftProcUIElement
{
    protected Vector2 tl;
    protected Color baseColor;
    protected GenerateBasicElements generator;
    protected SpellcraftProcUI procUI;
    
    public List<GameObject> Elements { get; set; } = new List<GameObject>();
    
    public SpellcraftProcUIElement(Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI)
    {
        this.baseColor = baseColor;
        this.generator = generator;
        this.procUI = procUI;
    }
    
    public abstract GameObject[] GenerateUI(Vector2 tl, out Vector2 offsets);

    public abstract void Refresh();
}