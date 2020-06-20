using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetCollor(this GameObject g, Color color)
    {
        g.GetComponent<Renderer>().material.color = color;
    }
}
