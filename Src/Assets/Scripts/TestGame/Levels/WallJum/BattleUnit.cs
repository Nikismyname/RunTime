using System;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    private int health = 100;
    private string team;

    public void SetUp(string team /*UnitDeadDelegate deadAction*/)
    {
        this.team = team;
        //this.UnitDeadEvent += deadAction;
    }

    //public delegate void UnitDeadDelegate(BattleUnit unit);
    //public event UnitDeadDelegate UnitDeadEvent;  

    public void Shoot(Vector3 position, Vector3 direction, string type)
    {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        p.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        p.transform.position = position;
        Projectile pScript = p.AddComponent<Projectile>();
        pScript.SetUp(20, direction);
        p.tag = this.team;
        p.layer = LayerMask.NameToLayer(this.team);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != this.team)
        {
            var projectile = collision.gameObject.GetComponent<Projectile>(); 
            if(projectile == null)
            {
                Debug.Log("Projectile Problems!");
                return;
            }

            this.health -= projectile.demage;
            this.DeathCheck();
        }
        else
        {
            Debug.Log("Friendly Fire");
        }
    }

    private void DeathCheck()
    {
        if(this.health <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}

