using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSelector
{
    /// The currently selected Target
    private GameObject target { get; set; }
    private List<GameObject> targets { get; set; }

    public TargetSelector(List<GameObject> targets)
    {
        this.targets = targets;
    }

    /// <summary>
    /// Registers selection change. New target has been clicked.
    /// </summary>
    /// <param name="id">The id of the selected target.</param>
    public void RegisterSelection(int id)
    {
        /// Finding the target.
        this.target = this.targets.SingleOrDefault(x => x.GetComponent<TargetBehaviour>().id == id);

        /// Telling all the targets to change their color according to the the id of the selected target. 
        /// Previously selected target will change to default color, currently selected will hange to select color
        foreach (GameObject target in this.targets)
        {
            target.GetComponent<TargetBehaviour>().VisualiseSelection(id);
        }

        /// Informing the action button UI about the change of target, so it displays the UI for current target.
        ReferenceBuffer.Instance.ManageProcUI.SetTarget(this.target);
    }

    /// <summary>
    /// This is called from the onPointDown event in <see cref="TargetBehaviour">. 
    /// The deselection login is in TargetBehaviour, this method only sets current Target to null.
    /// </summary>
    /// <param name="id">The id of delected target</param>
    public void RegisterDeselection(int id)
    {
        if (this.target.GetComponent<TargetBehaviour>().id != id)
        {
            Debug.Log("Deselection Error!");
        }

        this.target = null;
    }

    public GameObject Target => this.target;
}

