using UnityEngine;
using UnityEngine.UI;

public class Tutiral1StartMethod1 : LevelBase
{
    private GameObject target;
    private GameObject goal;
    private ReferenceBuffer rb;

    private void Start()
    {
        this.rb = ReferenceBuffer.Instance;
        rb.gl.CylinderBasePrefabStand1();
        var player = rb.gl.PlayerWithCamStand1();
        rb.InfoTextObject.GetComponent<Text>().text = ProblemDesctiptions.Tutorial1StartMethod1;
        rb.ShowCode.SetText(InitialCodes.Tutorial1StartMethod1);

        this.target = rb.gl.GenerateEntity(EntityType.Target, new Vector3(0, 0, 0), PrimitiveType.Cube, Color.gray, null, "Target");
        this.goal = rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(0, 8, 0), PrimitiveType.Cube, Color.blue, null, "Goal");
    }

    private void Update()
    {
        if((this.target.transform.position - this.goal.transform.position).magnitude < 1f)
        {
            this.rb.LevelManager.Success();
        }
    }

    public void ResetLevel()
    {
        
    }
}
