using UnityEngine;

public interface IUndoItem
{
    void Revert(PrimitiveObjectDataModifier pdom); 
}

