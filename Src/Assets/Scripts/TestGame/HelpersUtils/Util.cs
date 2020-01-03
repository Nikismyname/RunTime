using UnityEngine;

public static class Util
{
    public static NormalVariants GetNormalDerivatives(Vector3 normal, float XDegrees)
    {
        Vector3 leftFromNormal = Quaternion.AngleAxis(90, Vector3.up) * normal;
        Vector3 rightFromNormal = Quaternion.AngleAxis(-90, Vector3.up) * normal;
        Vector3 upFromNormal = Quaternion.AngleAxis(-90, leftFromNormal) * normal;

        Vector3 rightXDegrees = Quaternion.AngleAxis(XDegrees, normal) * upFromNormal;
        Vector3 leftXDegrees = Quaternion.AngleAxis(-XDegrees, normal) * upFromNormal;

        return new NormalVariants
        {
            Normal = normal,
            Left = leftFromNormal,
            Right = rightFromNormal,
            Up = upFromNormal,
            LeftXDegrees = leftXDegrees,
            RightXDegrees = rightXDegrees,
        };
    }

    public static void DrawLine(Vector3 anchorPoint, Vector3 line, Color color, int staySeconds = 4)
    {
        Debug.DrawLine(anchorPoint, anchorPoint + line.normalized * 4, color, staySeconds);
    }
}

public class NormalVariants
{
    public Vector3 Normal { get; set; }

    public Vector3 Left { get; set; }

    public Vector3 Right { get; set; }

    public Vector3 Up { get; set; }

    public Vector3 LeftXDegrees { get; set; }

    public Vector3 RightXDegrees { get; set; }
}
