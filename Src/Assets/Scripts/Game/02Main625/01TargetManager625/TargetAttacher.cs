using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetAttacher
{
    /// All attached monos per Target
    private Dictionary<GameObject, List<TargetManagerMonoWithNameAndMethods>> attachedMonos = new Dictionary<GameObject, List<TargetManagerMonoWithNameAndMethods>>();

    private GameObject target;

    public TargetAttacher(GameObject target)
    {
        this.target = target;
    }

    #region ATTACH MONO

    /// <summary>
    /// Attaches a runtime generated mono to a given target. 
    /// </summary>
    /// <param name="target">Target to which to attach the runtime mono.</param>
    /// <param name="funcs">Mono information: name, methods and pramaters.</param>
    /// <returns></returns>
    public MonoBehaviour AttachRuntimeMono(GameObject target, CompMethodsInAssemblyType funcs, string source)
    {
        /// Attaches the given mono and registeres in attachedMonos
        var monoData = this.AttachAndAddToDict(funcs, target, null);

        /// If a mono with the same name exists and the method singnature changed send it to the UI to redraw it.
        if (monoData.changesInMethodSignature)
        {
            ReferenceBuffer.Instance.ManageProcUI.RegisterNewOrChangedMono(this.GenerateButtonInformation(monoData, target, source));
            monoData.changesInMethodSignature = false;
        }

        return monoData.Mono;
    }

    /// <summary>
    /// Registers a compile time mono to given target. 
    /// </summary>
    /// <param name="target">Target to which to attach the runtime mono.</param>
    /// <param name="funcs">Mono information: name, methods and pramaters.</param>
    /// <returns></returns>
    public MonoBehaviour AttachCompiletimeMono(GameObject target, CompMethodsInAssemblyType funcs, MonoBehaviour mono, string source)
    {
        /// Attaches the given mono and registeres in attachedMonos
        var monoData = this.AttachAndAddToDict(funcs, target, mono);

        /// If a mono with the same name exists and the method singnature changed send it to the UI to redraw it.
        if (monoData.changesInMethodSignature)
        {
            ReferenceBuffer.Instance.ManageProcUI.RegisterNewOrChangedMono(this.GenerateButtonInformation(monoData, target, source));
            monoData.changesInMethodSignature = false;
        }
        return null;
    }

    #endregion

    /// <summary>
    /// Does two things: 
    ///     If attach is true, it attaches the mono to the target and registeres it in attachedMonos
    ///     If attach is false, means that the script is already attached and it only regerteres it in the same collection.
    /// Also if there is a mono with that name already, it remeves the old one and registeres the new one. 
    /// </summary>
    /// <param name="funcs"></param>
    /// <param name="target"></param>
    /// <param name="attach"></param>
    /// <param name="alreadyAttachedMono"> If null we need to attach what is passed. If not null - this is the mono that is already attached!</param>
    /// <returns></returns>
    private TargetManagerMonoWithNameAndMethods AttachAndAddToDict(
        CompMethodsInAssemblyType funcs,
        GameObject target,
        MonoBehaviour alreadyAttachedMono)
    {
        this.CreateMonoDataIfNoneExists(target);

        /// Getting exsiting monos for target.
        List<TargetManagerMonoWithNameAndMethods> existingMonosForTarget = this.attachedMonos[target];

        TargetManagerMonoWithNameAndMethods preexistingMono = GetPreexistingMonoWithSameName(target, funcs, existingMonosForTarget);

        this.DestroyIfMonoWithSameNameAttached(preexistingMono, target, funcs, existingMonosForTarget);

        MonoBehaviour attachedMono = alreadyAttachedMono == null ? funcs.Attach(target) : alreadyAttachedMono;

        TargetManagerMonoWithNameAndMethods newMonoData = new TargetManagerMonoWithNameAndMethods(funcs.TypeName, attachedMono);

        newMonoData.Methods = funcs.MethodInfos.Select(x => new TargetManagerMethodInfoWithName
        {
            MethodInfo = x.Value.MethodInfo,
            Parameters = x.Value.Parameters,
            Name = x.Value.MethodInfo.Name,
        }).ToList();

        newMonoData.changesInMethodSignature = this.AreThereChangesInSignature(newMonoData, preexistingMono);

        /// Adding it to the collection for the given target.
        existingMonosForTarget.Add(newMonoData);

        /// Passing the 
        if (alreadyAttachedMono == null)
        {
            ReferenceBuffer.Instance.Level.RegisterUpdatedMono(newMonoData);
        }

        return newMonoData;
    }

    private bool AreThereChangesInSignature(TargetManagerMonoWithNameAndMethods newMonoData, TargetManagerMonoWithNameAndMethods preexistingMono)
    {
        ///If this is the first attach 
        if (preexistingMono == null || preexistingMono.Methods == null)
        {
            return true;
        }

        if (!this.TwoMehtodCollHaveSameSignatures(preexistingMono.Methods, newMonoData.Methods))
        {
            return false;
        }

        return true;
    }

    private void CreateMonoDataIfNoneExists(GameObject target)
    {
        /// If data for the target does not exist yet, create it.
        if (!attachedMonos.ContainsKey(target))
        {
            this.attachedMonos[target] = new List<TargetManagerMonoWithNameAndMethods>();
        }
    }

    #region PART ONE

    private void DestroyIfMonoWithSameNameAttached(TargetManagerMonoWithNameAndMethods preexistingMono, GameObject target, CompMethodsInAssemblyType funcs, List<TargetManagerMonoWithNameAndMethods> existingMonosForTarget)
    {
        /// If a mono with the same name is attched, we destroy the old one!
        if (preexistingMono != null)
        {
            Component monoToDestroy = target.GetComponent(preexistingMono.Mono.GetType());
            GameObject.Destroy(monoToDestroy);
            existingMonosForTarget.Remove(existingMonosForTarget.SingleOrDefault(x => x.Name == funcs.TypeName));
            Debug.Log("Old Script is overriden");
        }
    }

    private TargetManagerMonoWithNameAndMethods GetPreexistingMonoWithSameName(GameObject target, CompMethodsInAssemblyType funcs, List<TargetManagerMonoWithNameAndMethods> existingMonosForTarget)
    {
        /// Cheking if there is more than one script with given name already registered which should never happen.
        TargetManagerMonoWithNameAndMethods[] existingMonos = existingMonosForTarget.Where(x => x.Name == funcs.TypeName).ToArray();

        if (existingMonos.Length > 1)
        {
            Debug.Log($"There are more that 1 scripts with name {funcs.TypeName} already attached!");
            Debug.Break();
            return null;
        }

        if (existingMonos.Length == 0)
        {
            return null;
        }

        ///We have exactly one
        TargetManagerMonoWithNameAndMethods previousMono = existingMonos[0];
        return previousMono;
    }

    #endregion 

    public Dictionary<GameObject, List<TargetManagerMonoWithNameAndMethods>> AttachedMonos => this.attachedMonos;

    #region HELPERS

    private bool TwoMehtodCollHaveSameSignatures(List<TargetManagerMethodInfoWithName> lOne, List<TargetManagerMethodInfoWithName> lTwo)
    {
        if (lOne.Count != lTwo.Count)
        {
            return false;
        }

        for (int i = 0; i < lOne.Count; i++)
        {
            var one = lOne[i];
            var two = lTwo[i];

            if (one.Name != two.Name)
            {
                return false;
            }

            if (one.Parameters.Length != two.Parameters.Length)
            {
                return false;
            }

            for (int j = 0; j < one.Parameters.Length; j++)
            {
                var pOne = one.Parameters[j];
                var pTwo = two.Parameters[j];

                if (pOne.Name != pTwo.Name || pOne.Type != pTwo.Type)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private UiMonoWithMethods GenerateButtonInformation(TargetManagerMonoWithNameAndMethods mono, GameObject incTarget = null, string source = "")
    {
        GameObject target = null;

        if (incTarget != null)
        {
            target = incTarget;
        }
        else
        {
            target = this.target;
        }

        if (target == null)
        {
            Debug.Log("GenButtons No Target!");
            return null;
        }

        var result = new UiMonoWithMethods
        {
            MonoName = mono.Name,
            Object = target,
            Methods = mono.Methods.Select(y => new UiMethodNameWithParameters
            {
                Name = y.Name,
                Parameters = y.Parameters,
            }).ToArray(),
            Source = source,
        };

        return result;
    }

    #endregion
}

