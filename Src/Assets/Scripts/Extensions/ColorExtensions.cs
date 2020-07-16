using UnityEngine;

public static class ColorExtensions
{
    public static Color SetAlpha(this Color color, float a = 0.6588235f)
    {
        color.a = a;
        return color;
    }
}

