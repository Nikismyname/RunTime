using UnityEngine;

public class Projectile: MonoBehaviour
{
    public int demage;
    private Vector3 direction;
    private float time = 0;
    private float maxTime = 10;

    public void SetUp(int demage, Vector3 direction)
    {
        this.demage = demage;
        this.direction = direction.normalized; 
    }
}
