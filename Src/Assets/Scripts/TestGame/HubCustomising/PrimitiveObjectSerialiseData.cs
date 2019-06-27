using UnityEngine;

public class PrimitiveObjectSerialiseData
{
    public Vector3 position;
    public Vector3 scale;
    public Vector3 rotation;
    public PrimitiveType type;
    public Color color;

    public PrimitiveObjectSerialiseData() { }

    public PrimitiveObjectSerialiseData(
        Vector3 position,
        Vector3 scale,
        Vector3 rotation,
        PrimitiveType type,
        Color color)
    {
        this.position = position;
        this.scale = scale;
        this.rotation = rotation;
        this.type = type;
        this.color = color;
    }
}
