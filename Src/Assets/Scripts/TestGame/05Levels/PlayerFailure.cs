using UnityEngine;

public class PlayerFailure: MonoBehaviour
{
    private LevelManager lm; 

    private void Start()
    {
        this.lm = GameObject.Find("Main").GetComponent<LevelManager>();
    }

    private void Update()
    {
        if(transform.position.y< -5)
        {
            //Debug.Log("Player Script detected fall!");
            lm.Failure("Fall");
        }
    }
}
