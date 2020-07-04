using UnityEngine;

public class UndoScaleChange: IUndoItem
{
    private Vector3 prev;
    private Vector3 curr;

    public UndoScaleChange(Vector3 prev, Vector3 curr)
    {
        this.prev = prev;
        this.curr = curr;
    }

    public void Revert(PrimitiveObjectDataModifier pdom)
    {
        pdom.scale = this.prev;
        pdom.gameObject.transform.localScale = pdom.scale;
    }
}

