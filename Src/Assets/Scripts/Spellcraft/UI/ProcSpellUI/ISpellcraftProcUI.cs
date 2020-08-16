using UnityEngine;

public interface ISpellcraftProcUI
{
    GameObject GetGameObject();
    void Setup(Camera cameraIn, ConnectionsTracker trackerIn, float globalScaleIn = 0.2f);
    void SetCanvasPosition(Vector3 pos);
}