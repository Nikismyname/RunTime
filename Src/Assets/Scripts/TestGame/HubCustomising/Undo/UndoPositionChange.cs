using UnityEngine;

public class UndoPositionChange : IUndoItem
{
    private Vector3 prev;
    private Vector3 curr;

    public UndoPositionChange(Vector3 prev, Vector3 curr)
    {
        this.prev = prev;
        this.curr = curr;
    }

    public void Revert(PrimitiveObjectDataModifier pdom)
    {
        pdom.position = this.prev;
        pdom.gameObject.transform.position = pdom.position;
    }
}

