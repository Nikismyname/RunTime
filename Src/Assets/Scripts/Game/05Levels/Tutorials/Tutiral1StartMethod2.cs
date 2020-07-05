using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutiral1StartMethod2 : LevelBase
{
    private GameObject target;
    private List<GameObject> goals = new List<GameObject>();
    private ReferenceBuffer rb;

    private void Start()
    {
        this.rb = ReferenceBuffer.Instance;
        rb.gl.CylinderBasePrefabStand1();
        var player = rb.gl.PlayerWithCamStand1();
        rb.InfoTextObject.GetComponent<Text>().text = ProblemDesctiptions.Tutorial1StartMethod2;
        rb.ShowCode.SetText(InitialCodes.Tutorial1StartMethod2);

        int baseY = 4;
        int baseX = 0;
        int baseZ = 0;
        int dist = 4;

        this.target = rb.gl.GenerateEntity(EntityType.Target, new Vector3(0, baseY, 0), PrimitiveType.Cube, Color.gray, null, "Target");

        this.goals.Add(rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(baseX, baseY + dist, baseZ), PrimitiveType.Cube, Color.blue, null, "Goal"));
        this.goals.Add(rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(baseX, baseY - dist, baseZ), PrimitiveType.Cube, Color.blue, null, "Goal"));

        this.goals.Add(rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(baseX - dist, baseY, baseZ), PrimitiveType.Cube, Color.blue, null, "Goal"));
        this.goals.Add(rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(baseX + dist, baseY, baseZ), PrimitiveType.Cube, Color.blue, null, "Goal"));

        this.goals.Add(rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(baseX, baseY, baseZ - dist), PrimitiveType.Cube, Color.blue, null, "Goal"));
        this.goals.Add(rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(baseX, baseY, baseZ + dist), PrimitiveType.Cube, Color.blue, null, "Goal"));
    }

    private void Update()
    {

        if(this.goals.Count == 0)
        {
            this.rb.LevelManager.Success(); 
        }

        int count = this.goals.Count; 

        for (int i = count - 1; i >= 0; i--)
        {
            GameObject currGoal = this.goals[i];
            if((currGoal.transform.position - this.target.transform.position).magnitude <= 1f)
            {
                currGoal.SetColor(Color.red);
                this.goals.Remove(currGoal);
            }
        }
    }

    public void ResetLevel()
    {

    }
}
