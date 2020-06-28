using System.Threading.Tasks;
using UnityEngine;

public class Tutiral3GOManip2 : LevelBase
{
    private void Start()
    {
        ReferenceBuffer.Instance.gl.CylinderBasePrefabStand1();
        var player = ReferenceBuffer.Instance.gl.PlayerWithCamStand1();
        ReferenceBuffer.Instance.ShowCode.SetText(this.BlankCode);
        ReferenceBuffer.Instance.gl.GenerateEntity(EntityType.Target, new Vector3(0, 5, 0), PrimitiveType.Cube, Color.gray, null, "Some");
        ReferenceBuffer.Instance.gl.CreateTaskDescription(this.ProblemDesciption);
        this.CheckForSuccess();
    }

    private async void CheckForSuccess()
    {
        while (true)
        {
            var b10 = GameObject.Find("Ben10");

            if (b10 != null && b10.transform.localScale.y == 2 && b10.transform.localScale.x == 1 && b10.transform.localScale.z == 1)
            {
                ReferenceBuffer.Instance.LevelManager.Success();
            }

            await Task.Delay(500);
        }
    }

    public override string ProblemDesciption { get; set; } = "Create Ben10 who is one 2 meter tall and 1 meter in the other two demenetions!";

    public override string SolutionCode { get; set; } =
@"
using UnityEngine;

public class Solution : MonoBehaviour
{
    private void Start()
    {
        var Ben10 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Ben10.transform.localScale = new Vector3(1,2,1);
        Ben10.name = ""Ben10"";
    }

    private void Update()
    {

    }
}
";
}
