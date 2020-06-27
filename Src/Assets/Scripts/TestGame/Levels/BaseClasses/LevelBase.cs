using UnityEngine;

public class LevelBase : MonoBehaviour, ILevelMain
{
    public virtual void RegisterUpdatedMono(MainMonoWithName data)
    {
    }

    public virtual void ResetLevel()
    {
        ReferenceBuffer.Instance.PlayerObject.transform.position = new Vector3(0,0,0);
        ReferenceBuffer.Instance.MySceneManager.SameLevel();
    }
}
