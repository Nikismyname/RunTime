using UnityEngine;

public static class Colors
{
    /// Base Cylinder Prefab Color 
    public static Color32 BaseColor { get; set; } = new Color32(0, 248, 170, 154);
    /// UI
    public static Color32 ActionButtonColor { get; set; } = new Color32(229, 200, 25, 255);
    public static Color32 CsFileButtonColor { get; set; } = Color.white;

    /// Tiles
    public static Color32 TileDefault { get; set; } = Color.green;
    public static Color32 TileStepedOn { get; set; } = Color.cyan;
    public static Color32 TileSelected { get; set; } = Color.red;
    public static Color32 OrientationTile { get; set; } = Color.black;
}

