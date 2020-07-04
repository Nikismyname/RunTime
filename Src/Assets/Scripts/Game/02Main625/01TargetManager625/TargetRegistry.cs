using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetRegistry
{   
    /// List of all Targets
    private List<GameObject> targets = new List<GameObject>();

    /// <summary>
    /// Registers new target. Eather a standart target or test target. 
    /// Poth can be selected and scripts attached to them, but different kinds. 
    /// Standart targets accept Monos with attach funcs that modify the behaviour
    /// of the target. Tests accept scripts (not monos) with a solve method and return bool 
    /// which indicates if the test passed.
    /// </summary>
    /// <param name="newTarget">The new target itself</param>
    /// <param name="testName">If it is a test target this is the name of the test they are for.</param>
    public void RegisterTarget(GameObject newTarget, TargetType targetType, string testName = "")
    {
        TargetBehaviour tb = newTarget.GetComponent<TargetBehaviour>();

        if (tb == null)
        {
            Debug.Log("The target you are trying to register does not have TargetBehaviour!");
            Debug.Break();
            return;
        }

        int newId = 0;

        if (this.targets.Count > 0)
        {
            newId = this.targets.Select(x => x.GetComponent<TargetBehaviour>()).Max(x => x.id) + 1;
        }

        tb.SetUp(newId, targetType, testName);
        this.targets.Add(newTarget);
    }

    /// <summary>
    /// This is currently only called from ResetLevel function of <see cref="Level1Main">. 
    /// It simply removes the date for the Target GamoObject from memory.
    /// </summary>
    /// <param name="obj"></param>
    public void UnregisterTarget(GameObject obj)
    {
        var tb = obj.GetComponent<TargetBehaviour>();

        if (tb == null)
        {
            return;
        }

        this.targets = this.targets.Where(x => x.GetComponent<TargetBehaviour>().id != tb.id).ToList();
    }

    public List<GameObject> Targets => this.targets;
}
