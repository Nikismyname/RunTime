using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TargetAttacher
{
    /// All attached monos per Target
    private Dictionary<GameObject, List<MainMonoWithName>> attachedMonos = new Dictionary<GameObject, List<MainMonoWithName>>();

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
        var monoData = this.AttachAndAddToDict(funcs, target, true, null);

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
        var monoData = this.AttachAndAddToDict(funcs, target, false, mono);

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
    private MainMonoWithName AttachAndAddToDict(
        CompMethodsInAssemblyType funcs,
        GameObject intTarget,
        bool attach,
        MonoBehaviour incMono)
    {
        this.CreateDataIfNotExist(intTarget);

        this.DestroyIfMonoWithSameNameAttached(intTarget, funcs, out int newVersion, out List<MainMethodInfoWithName> previousMethods, out List<MainMonoWithName> monoList, out bool preExistionMono); 

        /// Managing the mono, if we provide one that is already attached we do not need to attach but still register.
        MonoBehaviour script = null;
        if (attach == false)
        {
            if (incMono == null)
            {
                Debug.Log("No Attach but the sent script is null!");
            }

            script = incMono;
        }
        else
        {
            script = funcs.Attach(intTarget);
        }

        var newMonoData = new MainMonoWithName(funcs.TypeName, script);
        newMonoData.MyMethods = funcs.MethodInfos.Select(x => new MainMethodInfoWithName
        {
            MethodInfo = x.Value.MethodInfo,
            Parameters = x.Value.Parameters,
            Name = x.Value.MethodInfo.Name,
        }).ToList();

        newMonoData.Version = newVersion;
        if (preExistionMono == false)
        {
            /// Since it is just created, the signature has changed. 
            newMonoData.changesInMethodSignature = true;
        }
        else
        {
            if (previousMethods == null)
            {
                Debug.Log("Failed at collecting the previous signature");
                Debug.Break();
                return null;
            }

            /// Comparing the two methods' signitures.
            if (this.TwoMehtodCollHaveSameSignatures(previousMethods, newMonoData.MyMethods))
            {
                newMonoData.changesInMethodSignature = false;
            }
            else
            {
                newMonoData.changesInMethodSignature = true;
            }
        }

        /// Adding it to the collection for the given target.
        monoList.Add(newMonoData);

        if (attach == true)
        {
            ReferenceBuffer.Instance.Level.RegisterUpdatedMono(newMonoData);
        }

        return newMonoData;
    }

    #region CREATE_INITIAL_DATA
    public void CreateDataIfNotExist(GameObject intTarget)
    {
        /// If data for the target does not exist yet, create it.
        if (!attachedMonos.ContainsKey(intTarget))
        {
            this.attachedMonos[intTarget] = new List<MainMonoWithName>();
        }
    }
    #endregion

    #region PART ONE

    private void DestroyIfMonoWithSameNameAttached(GameObject intTarget, CompMethodsInAssemblyType funcs, out int version, out List<MainMethodInfoWithName> prevMethods, out List<MainMonoWithName> mList, out bool preExistMono)
    {
        version = default;
        prevMethods = default;
        mList = default;
        preExistMono = default;

        /// Getting exsiting monos for target.
        List<MainMonoWithName> monoList = this.attachedMonos[intTarget];

        /// Cheking if there is more than one script with given name already registered
        /// which should never happen.
        MainMonoWithName[] existingMonos = monoList.Where(x => x.Name == funcs.TypeName).ToArray();
        if (existingMonos.Length > 1)
        {
            Debug.Log($"There are more that 1 scripts with name {funcs.TypeName} already attached!");
            Debug.Break();
            return;
        }

        var newVersion = 0;
        List<MainMethodInfoWithName> previousMethods = null;
        var preExistionMono = false;

        /// If a mono with the same name is attched, we destroy the old one!
        if (existingMonos.Length == 1)
        {
            preExistionMono = true;
            var preexistingMonoWithName = existingMonos[0];
            ///Updating the version.
            newVersion = preexistingMonoWithName.Version + 1;
            previousMethods = preexistingMonoWithName.MyMethods;

            var monoToDestroy = intTarget.GetComponent(preexistingMonoWithName.Mono.GetType());
            GameObject.Destroy(monoToDestroy);
            monoList.Remove(monoList.SingleOrDefault(x => x.Name == funcs.TypeName));
            Debug.Log("Old Script is overriden");
        }

        version = newVersion;
        prevMethods = previousMethods;
        mList = monoList;
        preExistMono = preExistionMono; 
    }

    #endregion 

    public Dictionary<GameObject, List<MainMonoWithName>> AttachedMonos => this.attachedMonos;

    #region HELPERS

    private bool TwoMehtodCollHaveSameSignatures(List<MainMethodInfoWithName> lOne, List<MainMethodInfoWithName> lTwo)
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

    private UiMonoWithMethods GenerateButtonInformation(MainMonoWithName mono, GameObject incTarget = null, string source = "")
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
            Methods = mono.MyMethods.Select(y => new UiMethodNameWithParameters
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

