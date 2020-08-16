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

    public virtual GameObject[] GenerateUI(Vector2 tl, out Vector2 offsets)
    {
        this.tl = tl;
        //Debug.Log(this.tl.ToString() + $" {this.GetType().Name}");
        this.Cleanup();
        GameObject[] results = this.GenerateUI(out Vector2 offset);
        offsets = offset;
        this.CorrectForParentMove();
        return results;
    }

    protected abstract GameObject[] GenerateUI(out Vector2 offsets);

    public virtual void Refresh()
    {
        this.GenerateUI(this.tl, out _);
    }

    public virtual void Cleanup()
    {
        foreach (var item in this.Elements)
        {
            GameObject.Destroy(item);
        }

        this.Elements = new List<GameObject>();
    }

    protected void CorrectForParentMove()
    {
        foreach (var item in this.Elements)
        {
            item.transform.position += this.procUI.canvasGO.transform.position;
        }
    }
}