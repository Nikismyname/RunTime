using System.Collections.Generic;
using UnityEngine;

public abstract class SpellcraftProcUIElement
{
    public Vector2 Tl { get; set; }
    public Color BaseColor { get; set; }

    public List<GameObject> Elements { get; set; }

    public GenerateBasicElements Generator { get; set; }

    public SpellcraftProcUI ProcUI { get; set; }
    
    public SpellcraftProcUIElement(Vector2 tl, Color baseColor, GenerateBasicElements generator, SpellcraftProcUI procUI)
    {
        this.BaseColor = baseColor;
        this.Tl = tl;
        this.Generator = generator;
        this.ProcUI = procUI;
    }
    
    public abstract GameObject[] GenerateUI(out Vector2 offsets);

    public abstract void Refresh();
}