using System.Collections.Generic;
using UnityEngine;

public class PrimitiveObjectDataModifier : MonoBehaviour
{
    public Vector3 position;
    public Vector3 scale;
    public Vector3 rotation;
    public PrimitiveType type;
    public Color color;

    public bool dirty = false;
    public MeshRenderer myRenderer;
    private Stack<IUndoItem> undos; 

    private void Start()
    {
        this.myRenderer = this.gameObject.GetComponent<MeshRenderer>();
        this.undos = new Stack<IUndoItem>();
    }

    //public void SetUp(
    //    Vector3 position,
    //    Vector3 scale,
    //    Vector3 rotation,
    //    PrimitiveType type,
    //    Color color
    //)
    //{
    //    this.position = position;
    //    this.scale = scale;
    //    this.rotation = rotation;
    //    this.type = type;
    //    this.color = color;
    //}

    public void SetUp(PrimitiveObjectSerialiseData data)
    {
        this.position = data.position;
        this.scale = data.scale;
        this.rotation = data.rotation;
        this.type = data.type;
        this.color = data.color;
    }

    public void SetPosition(Vector3 newPosition)
    {
        var init = this.position;

        this.position = newPosition;
        this.gameObject.transform.position = this.position;

        this.undos.Push(new UndoPositionChange(init,this.position));
        this.dirty = true;
    }

    public void OffsetPosition(Vector3 offset)
    {
        var init = this.position; 

        this.position += offset;
        this.gameObject.transform.position = this.position;

        this.undos.Push(new UndoPositionChange(init, this.position));
        this.dirty = true;
    }

    public void SetScale(Vector3 newScale)
    {
        var init = this.scale; 

        this.scale = newScale;
        this.gameObject.transform.localScale = this.scale;

        this.undos.Push(new UndoScaleChange(init, this.scale));
        this.dirty = true;
    }

    public void SetRotation(Vector3 rotation)
    {
        var init = this.rotation; 

        this.rotation = rotation;
        this.gameObject.transform.eulerAngles = this.rotation;

        this.undos.Push(new UndoRotationChange(init, this.rotation)); 
        this.dirty = true; 
    } 

    public void SetColor(Color color)
    {
        var init = this.color;

        this.color = color;
        this.myRenderer.material.color = this.color;

        this.undos.Push(new UndoColorChange(init, this.color));
        this.dirty = true;
    }

    public void Undo()
    {
        if(this.undos.Count <= 0)
        {
            return; 
        }

        var undo = this.undos.Pop();

        undo.Revert(this); 
    }
}

