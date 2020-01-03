using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallCollisionStatusManager
{
    private List<WallCollisionStatus> wallCollisions = new List<WallCollisionStatus>();

    public void RegisterCollisionEnter(string name, Vector3 normal, string tag = "")
    {
        var existing = this.wallCollisions.SingleOrDefault(x => x.Name == name && x.Normal == normal);

        if (existing == null)
        {
            this.wallCollisions.Add(new WallCollisionStatus(normal, name, true) { Tag = tag });
        }
        else
        {
            existing.IsColliding = true;
        }
    }

    /// TODO: What if the normal changes while walking without leaving the wall! 
    public void RegisterCollisionExit(string name)
    {
        var existing = this.wallCollisions.Where(x => x.Name == name).ToArray();

        if (existing.Length > 0)
        {
            foreach (var item in existing)
            {
                item.IsColliding = false;
            }
        }
        else
        {
            Debug.LogError($"Collision Exit could not find the wall: {name}");
            Debug.Break();
        }
    }

    /// <summary>
    /// Returns the first normal of colliding object or null
    /// </summary>
    /// <returns></returns>
    public Vector3? GetCurrenNormal() => this.wallCollisions.FirstOrDefault(x => x.IsColliding == true)?.Normal; 

    /// <summary>
    /// Is the player colliding with any of the tracked objects.
    /// </summary>
    public bool IsColliding() => this.wallCollisions.Any(x=> x.IsColliding == true);
    
    /// <summary>
    /// Is the player colliding with object with specified name.
    /// </summary>
    public bool IsColliding(string name) => this.wallCollisions.Any(x=> x.Name == name && x.IsColliding == true); 
}

class WallCollisionStatus
{
    public WallCollisionStatus(Vector3 normal, string name, bool isColliding)
    {
        Normal = normal;
        Name = name;
        IsColliding = isColliding;
    }

    public Vector3 Normal { get; set; }

    public string Name { get; set; }

    public string Tag { get; set; }

    public bool IsColliding { get; set; }
}
