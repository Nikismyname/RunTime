using UnityEngine;
using UnityEngine.UI;

public class Tutiral2Vector1 : MonoBehaviour, ILevelMain
{
    private GameObject target;
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

        rb.gl.GenerateEntity(EntityType.NonTarget, new Vector3(baseX, baseY + dist, baseZ), PrimitiveType.Cube, Color.blue, null, "Goal");
    }

    private void Update()
    {

        //if(this.goals.Count == 0)
        //{
        //    this.rb.lm.Success(); 
        //}

        //int count = this.goals.Count; 

        //for (int i = count - 1; i >= 0; i--)
        //{
        //    GameObject currGoal = this.goals[i];
        //    if((currGoal.transform.position - this.target.transform.position).magnitude <= 1f)
        //    {
        //        currGoal.SetCollor(Color.red);
        //        this.goals.Remove(currGoal);
        //    }
        //}
    }

    public void ResetLevel()
    {

    }
}
