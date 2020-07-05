using UnityEngine;

public class LevelBase : MonoBehaviour, ILevelMain
{
    public virtual void RegisterUpdatedMono(TargetManagerMonoWithNameAndMethods data)
    {
    }

    public virtual void ResetLevel()
    {
        ReferenceBuffer.Instance.PlayerObject.transform.position = new Vector3(0, 0, 0);
        ReferenceBuffer.Instance.MySceneManager.SameLevel();
    }

    public virtual string BlankCode { get; set; } =
@"
using UnityEngine;

public class Solution : MonoBehaviour
{
    private void Start()
    {

    }

    private void Update()
    {

    }
}
";

    public virtual string SolutionCode { get; set; } =
@"
using UnityEngine;

public class Solution : MonoBehaviour
{
    private void Start()
    {

    }

    private void Update()
    {

    }
}
";

    public virtual string ProblemDesciption { get; set; } =
@"
Solve this shit!
";
}
