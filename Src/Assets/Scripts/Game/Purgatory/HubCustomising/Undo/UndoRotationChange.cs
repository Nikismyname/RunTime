using UnityEngine;

public class UndoRotationChange : IUndoItem
{
    private Vector3 prev;
    private Vector3 curr;

    public UndoRotationChange(Vector3 prev, Vector3 curr)
    {
        this.prev = prev;
        this.curr = curr;
    }

    public void Revert(PrimitiveObjectDataModifier pdom)
    {
        pdom.rotation = this.prev;
        pdom.gameObject.transform.eulerAngles = pdom.rotation;
    }
}

