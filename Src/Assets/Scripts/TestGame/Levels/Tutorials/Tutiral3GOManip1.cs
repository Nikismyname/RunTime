using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class Tutiral3GOManip1 : LevelBase
{
    private GameObject target;
    private GameObject context;
    private GameObject context2;

    private List<GameObject> goals = new List<GameObject>();
    private ReferenceBuffer rb;

    private void Start()
    {
        this.rb = ReferenceBuffer.Instance;
        rb.gl.CylinderBasePrefabStand1();
        var player = rb.gl.PlayerWithCamStand1();
        rb.InfoTextObject.GetComponent<Text>().text = ProblemDesctiptions.Tutorial1StartMethod2;
        //rb.ShowCode.SetText(InitialCodes.Tutorial1StartMethod2);
        rb.ShowCode.SetText("");

        this.target = rb.gl.GenerateEntity(EntityType.Target, new Vector3(0, 5, 0), PrimitiveType.Cube, Color.gray, null, "Some");

        this.context = rb.gl.GenerateEntity(EntityType.Context, new Vector3(0, 0, 0), PrimitiveType.Sphere, Color.white, null, "Josh12");
        this.context2 = rb.gl.GenerateEntity(EntityType.Context, new Vector3(0, 2, 0), PrimitiveType.Sphere, Color.white, null, "Josh13");

        this.context.SetContextText("My Name is \"Josh12\" I want to be Color.Red");
        this.context2.SetContextText("My Name is \"Josh13\" I want to be Higher");

        this.CheckForSuccess();
    }

    private async void CheckForSuccess()
    {
        while (true)
        {
            if (this.context.GetComponent<Renderer>().material.color == Color.red && context2.transform.position.y > 2)
            {
                ReferenceBuffer.Instance.LevelManager.Success();
            }

            await Task.Delay(500); 
        }
    }
}
