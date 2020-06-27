using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetCollor(this GameObject g, Color color)
    {
        g.GetComponent<Renderer>().material.color = color;
    }

    public static void SetContextText(this GameObject g, string text)
    {
        try
        {
            ContextBehaviour tb = g.GetComponent<ContextBehaviour>();
            tb.SetContextText(text);
        }
        catch
        {
            Debug.LogError("You DUM DUM!");
        }
    }
}
