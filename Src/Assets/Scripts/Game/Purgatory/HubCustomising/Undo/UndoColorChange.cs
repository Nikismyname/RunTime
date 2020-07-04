using UnityEngine;

public class UndoColorChange: IUndoItem
{
    private Color prev;
    private Color curr;

    public UndoColorChange(Color prev, Color curr)
    {
        this.prev = prev;
        this.curr = curr;
    }

    public void Revert(PrimitiveObjectDataModifier pdom)
    {
        pdom.color = this.prev;
        pdom.myRenderer.material.color = pdom.color;
    }
}

