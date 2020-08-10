using UnityEngine;

public class Teleporter
{
    public void Teleport(Vector3 newPosition)
    {
        ReferenceBuffer.Instance.PlayerObject.transform.position = newPosition;
    }
}