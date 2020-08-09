using System;
using System.Threading.Tasks;
using UnityEngine;

public class LineDestroyer
{
    public void CreateDestroyerLine(Vector3 begin, Vector3 end)
    {
        LineDrawer drawer = new LineDrawer(null);
        GameObject lineParent = drawer.DrawInGameLine(begin, end, Color.blue, 0.5f, new GameObject("Parent").transform);
        GameObject line = lineParent.transform.Find("line").gameObject; 
        line.GetComponent<CapsuleCollider>().isTrigger = true;
        line.AddComponent<LineDestroyerBehaviour>();
    }
}

public class LineDestroyerBehaviour : MonoBehaviour
{
    private bool ended = false;

    private void Start()
    {
        this.DelayEnd();
    }

    private async void DelayEnd()
    {
        await Task.Delay(100);
        ended = true;
    }

    private void Update()
    {
        if (this.ended == false)
        {
            return;
        }

        Debug.Log("LineDone");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HI!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        
        if (other.gameObject.CompareTag("destroy"))
        {
            Destroy(other.gameObject);
        }
    }
}