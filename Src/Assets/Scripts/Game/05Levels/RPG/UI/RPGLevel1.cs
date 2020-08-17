using UnityEngine;

public class RPGLevel1 : PRGLevelBase
{
    protected override void Start()
    {
        base.Start();

        /// level setup
        GameObject toDestroy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        toDestroy.tag = "destroy";
        toDestroy.transform.position = new Vector3(0, 0, 0);
        var rb = toDestroy.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        Debug.Log("all systems go");
        
    }
    
    public override void ResetLevel()
    {
        this.player.transform.position = new Vector3(0, 0, 10);
    }
}