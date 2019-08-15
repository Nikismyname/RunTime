using UnityEngine;

public class Level4Main : MonoBehaviour, ILevelMain
{
    private GameObject player; 

    private void Start()
    {
        var main = GameObject.Find("Main");
        var ms = main.GetComponent<Main>();
        var buffer = main.GetComponent<ReferenceBuffer>(); 
        var gl = new GenerateLevel(ms,buffer);
        var grid = gl.GenerateGrid(10,10);

        ///PLAYER
        this.player = gl.Player(new Vector3(20, 0, 10), true, true, true);
        var mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = mainCamera.GetComponent<CamHandling>();
        camHandling.target = player.transform;
    }

    public void ResetLevel()
    {
        this.player.transform.position = new Vector3(0, 1, 0);
    }
}

