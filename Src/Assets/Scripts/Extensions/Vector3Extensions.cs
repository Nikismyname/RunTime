
using UnityEngine;

public static class Vector3Extensions
{
    public static void DrawCrossUpdate(this Vector3 v, Color? color = null, float ? duration = null, float halfLength = 1f)
    {
        color = color == null ? Color.black : color.Value;
        duration = duration == null ? Time.deltaTime : duration.Value; 

        Debug.DrawLine(v + new Vector3(halfLength, 0, 0), v + new Vector3(-halfLength, 0, 0), color.Value, duration.Value); 
        Debug.DrawLine(v + new Vector3(0, halfLength, 0), v + new Vector3(0, -halfLength, 0), color.Value, duration.Value); 
        Debug.DrawLine(v + new Vector3(0, 0, halfLength), v + new Vector3(0, 0, -halfLength), color.Value, duration.Value); 
    }
}


