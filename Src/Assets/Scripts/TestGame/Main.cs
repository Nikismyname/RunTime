#region INIT
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible for: 
///     Registering targets. 
///     Tracking the currently selected target.
///     Registering monos for targets.
///     Comunicating current target and attached monos to the UI <see cref="ManageActionsButtons">
/// </summary>
public class Main : MonoBehaviour
{
    [SerializeField] public bool testMode;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject playerKinematicPrefab;
    [SerializeField] public GameObject cylinderPrefab;
    [SerializeField] public float playerSpeed = 0.0005f;
    [SerializeField] public float NSSAllSpeedMultipyer = 0.5f;
    [SerializeField] public float NSSAllSlowConstantsMultiplyer = 0.8f;

    [SerializeField] public float extraGravity;
    [SerializeField] public float jumpForce;
    [SerializeField] public float forceToVelocity = 0.05f;

    /// The currently selected Target
    public GameObject Target { get; set; }
    /// List of all Targets
    private List<GameObject> targets = new List<GameObject>();
    /// All attached monos per Target
    private Dictionary<GameObject, List<MainMonoWithName>> attachedMonos = new Dictionary<GameObject, List<MainMonoWithName>>();
    /// Reference to the script that manages action buttons 
    private ManageActionsButtons manageButtons;

    void Awake()
    {
        this.manageButtons = GameObject.Find("ActionsContent").GetComponent<ManageActionsButtons>();

        //if (this.testMode)
        //{
        //    this.GenerateTargetsTest();
        //}

        ///Every two seconds check for destroyed monos and send message to <see cref="ManageActionsButtons"> 
        ///to remove them from the UI. 
        InvokeRepeating("PruneDestroyedMonoBehaviurs", 1, 2);
    }
    #endregion

    #region TARGET_MANAGMENT
    /// <summary>
    /// Registers new target. Eather a standart target or test target. 
    /// Poth can be selected and scripts attached to them, but different kinds. 
    /// Standart targets accept Monos with attach funcs that modify the behaviour
    /// of the target. Tests accept scripts (not monos) with a solve method and return bool 
    /// which indicates if the test passed.
    /// </summary>
    /// <param name="newTarget">The new target itself</param>
    /// <param name="testName">If it is a test target this is the name of the test they are for.</param>
    public void RegisterTarget(GameObject newTarget, string testName = "")
    {
        var tb = newTarget.GetComponent<TargetBehaviour>();
        if (tb == null)
        {
            Debug.Log("The target you are trying to register does not have TargetBehaviour!");
            Debug.Break();
            return;
        }

        var newId = this.targets.Count;
        if (testName == "")
        {
            tb.SetUp(newId);
        }
        else
        {
            tb.SetUp(newId, TargetType.Test, testName);
        }
        this.targets.Add(newTarget);
    }

    /// <summary>
    /// Registers selection change. New target has been clicked.
    /// </summary>
    /// <param name="id">The id of the selected target.</param>
    public void RegisterSelection(int id)
    {
        /// Finding the target.
        this.Target = this.targets.SingleOrDefault(x => x.GetComponent<TargetBehaviour>().id == id);

        /// Telling all the targets to change their color according to the the id of the selected target. 
        /// Previously selected target will change to default color, currently selected will hange to select color
        foreach (var target in this.targets)
        {
            target.GetComponent<TargetBehaviour>().VisualiseSelection(id);
        }

        /// Informing the action button UI about the change of target, so it displays the UI for current target.
        this.manageButtons.SetTarget(this.Target);
    }

    /// <summary>
    /// This is called from the onPointDown event in <see cref="TargetBehaviour">. 
    /// The deselection login is in TargetBehaviour, this method only sets current Target to null.
    /// </summary>
    /// <param name="id">The id of delected target</param>
    public void RegisterDeselection(int id)
    {
        if (this.Target.GetComponent<TargetBehaviour>().id != id)
        {
            Debug.Log("Deselection Error!");
        }

        this.Target = null;
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
    #endregion

    #region ATTACH
    
    /// <summary>
    /// Attaches a runtime generated mono to a given target. 
    /// </summary>
    /// <param name="target">Target to which to attach the runtime mono.</param>
    /// <param name="funcs">Mono information: name, methods and pramaters.</param>
    /// <returns></returns>
    public MonoBehaviour AttachRuntimeMono(GameObject target, CompMethodsInAssemblyType funcs)
    {
        /// Attaches the given mono and registeres in <see cref="attachedMonos">
        var monoData = this.AttachAndAddToDict(funcs, target, true, null);

        /// If a mono with the same name exists and the method singnature changed send it to the UI to redraw it.
        if (monoData.changesInMethodSignature)
        {
            this.manageButtons.RegisterNewOrChangedMono(this.GenerateButtonInformation(monoData, target));
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
    public MonoBehaviour RegisterCompileTimeMono(GameObject target, CompMethodsInAssemblyType funcs, MonoBehaviour mono)
    {
        /// Attaches the given mono and registeres in <see cref="attachedMonos">
        var monoData = this.AttachAndAddToDict(funcs, target, false, mono);

        /// If a mono with the same name exists and the method singnature changed send it to the UI to redraw it.
        if (monoData.changesInMethodSignature)
        {
            this.manageButtons.RegisterNewOrChangedMono(this.GenerateButtonInformation(monoData, target));
            monoData.changesInMethodSignature = false;
        }
        return null; 
    }


    //public MonoBehaviour AttachMono(
    //    CompMethodsInAssemblyType funcs,
    //    bool toTarget = true,
    //    GameObject actTarget = null,
    //    bool attach = true,
    //    MonoBehaviour incMono = null)
    //{
    //    GameObject internalTarget = null;

    //    if (toTarget == false)
    //    {
    //        if (actTarget == null)
    //        {
    //            Debug.Log("You chose to privade custom target but sent null as its value!");
    //            Debug.Break();
    //        }

    //        internalTarget = actTarget;
    //    }
    //    else
    //    {
    //        internalTarget = this.Target;
    //    }

    //    var mono = this.AttachAndAddToDict(funcs, internalTarget, attach, incMono);

    //    if (mono.changesInMethodSignature)
    //    {
    //        this.manageButtons.RegisterNewOrChangedMono(this.GenerateButtonInformation(mono, internalTarget));
    //        mono.changesInMethodSignature = false;
    //    }

    //    return mono.Mono;
    //}

    /// <summary>
    /// Does tho things: 
    ///     If attach is true, it attaches the mono to the target and registeres it in <see cref="attachedMonos">.
    ///     If attach is false, means that the script is already attached and it only regerteres it in the same collection.
    /// Also if there is a mono with that name already, it remeves the old one and registeres the new one. 
    /// </summary>
    private MainMonoWithName AttachAndAddToDict(
        CompMethodsInAssemblyType funcs,
        GameObject intTarget,
        bool attach,
        MonoBehaviour incMono)
    {
        /// If data for the target does not exist yet, create it.
        if (!attachedMonos.ContainsKey(intTarget))
        {
            this.attachedMonos[intTarget] = new List<MainMonoWithName>();
        }

        /// Getting exsiting monos for target.
        var monoList = this.attachedMonos[intTarget];

        /// Cheking if there is more than one script with given name already registered
        /// which should never happen.
        var existingMonos = monoList.Where(x => x.Name == funcs.TypeName).ToArray();
        if (existingMonos.Length > 1)
        {
            Debug.Log($"There are more that 1 scripts with name {funcs.TypeName} already attached!");
            Debug.Break();
            return null;
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
            Destroy(monoToDestroy);
            monoList.Remove(monoList.SingleOrDefault(x => x.Name == funcs.TypeName));
            Debug.Log("Old Script is overriden");
        }

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

        return newMonoData;
    }

    public bool TwoMehtodCollHaveSameSignatures(List<MainMethodInfoWithName> lOne, List<MainMethodInfoWithName> lTwo)
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

    #endregion

    #region CALL_FUNCTION
    public void CallFunction(string monoName, string methodName, ParameterNameWithSingleObjectValues[] parameters)
    {
        if (this.Target == null)
        {
            Debug.Log("Select a target before calling function!");
            return;
        }

        if (!this.attachedMonos.ContainsKey(this.Target))
        {
            Debug.Log("The target has no behaviours attached!");
            return;
        }

        var monos = this.attachedMonos[this.Target];

        var wantedMonos = monos.Where(x => x.Name == monoName).ToArray();
        if (wantedMonos.Length != 1)
        {
            Debug.Log("wanted behaviour not found or more than one found");
            return;
        }

        var wantedMono = wantedMonos[0];

        var wantedMethods = wantedMono.MyMethods.Where(x => x.Name == methodName).ToArray();

        if (wantedMethods.Length != 1)
        {
            Debug.Log("wanted method not found or more than one found");
            return;
        }

        var wantedMethod = wantedMethods[0];

        var parameterObjects = this.GenerateParamaterObjects(parameters, wantedMethod.Parameters); //Generating Paramater Objects

        if (parameterObjects == null)
        {
            Debug.Log("Failed at generating paramater objects!");
            return;
        }

        wantedMethod.MethodInfo.Invoke(wantedMono.Mono, parameterObjects); //Invocation Here

        //Debug.Log($"Method {methodName} successfully invoked on mono {monoName}");
    }

    private object[] GenerateParamaterObjects(
        ParameterNameWithSingleObjectValues[] data,
        UiParameterWithType[] parameters)
    {
        /// Consolidating all input values that belong to single Parameter type 
        /// vecotr3 has all theree inputs for example
        /// vector2 has 2; int has 1; 
        var parametersWithInputs = new List<ParameterNameWithAllObjectValues>();

        foreach (var item in data)
        {
            var existingParamater = parametersWithInputs.Where(x => x.ParameterName == item.ParameterName).SingleOrDefault();
            if (existingParamater == null)
            {
                var newItem = new ParameterNameWithAllObjectValues
                {
                    ParameterName = item.ParameterName,
                    ParamaterValues = new List<object>()
                };

                parametersWithInputs.Add(newItem);
                existingParamater = newItem;
            }

            existingParamater.ParamaterValues.Add(item.ParameterValue);
        }
        ///.......................................................................................

        var result = new List<object>();

        foreach (var item in parametersWithInputs)
        {
            var paramName = item.ParameterName;

            var currentParamType = parameters.SingleOrDefault(x => x.Name == paramName)?.Type;

            if (currentParamType == null)
            {
                Debug.Log("Could not find the type for a property!");
                Debug.Break();
            }

            var values = item.ParamaterValues;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == "")
                {
                    values[i] = "0";
                    Debug.Log("You left input empty, there is 0 there now!");
                }
            }

            //VECTOR3
            if (currentParamType == typeof(Vector3))
            {
                if (values.Count != 3)
                {
                    Debug.Log("Vector 3 does not have 3 inputs");
                    return null;
                }

                float x, y, z; x = y = z = 0f;

                try
                {
                    x = (float)ConvertToAny(values[0], typeof(float), false);
                    y = (float)ConvertToAny(values[1], typeof(float), false);
                    z = (float)ConvertToAny(values[2], typeof(float), false);
                }
                catch
                {
                    Debug.Log("Failed at parsing vector3 floats!");
                    return null;
                }

                var vec = new Vector3(x, y, z);

                result.Add(vec);
            }
            else if (currentParamType == typeof(Vector2)) //VECTOR 2
            {
                if (values.Count != 2)
                {
                    Debug.Log("Vector2 does not have 2 inputs");
                    return null;
                }
                float x, y; x = y = 0f;

                try
                {
                    x = (float)ConvertToAny(values[0], typeof(float), false);
                    y = (float)ConvertToAny(values[1], typeof(float), false);
                }
                catch
                {
                    Debug.Log("Failed at parsing vector2 floats!");
                    return null;
                }

                var vec = new Vector2(x, y);

                result.Add(vec);
            }
            else
            {
                if (values.Count != 1)
                {
                    Debug.Log("Single input params do not have 1 input!");
                    return null;
                }

                var value = values[0]; // for single input parameters

                if (currentParamType == typeof(Color))
                {
                    result.Add(value);
                }
                else
                {
                    var convertedValue = this.ConvertToAny(value, currentParamType);

                    if (convertedValue == null)
                    {
                        return null;
                    }

                    result.Add(convertedValue);
                }
            }
        }

        return result.ToArray();
    }

    private object ConvertToAny(object value, Type to, bool catchError = true)
    {
        if (catchError)
        {
            try
            {
                return Convert.ChangeType(value, to);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed at conterting {value} to {to.Name}");
                Debug.Log($"Exception: {e.Message}");
                return null;
            }
        }
        else
        {
            return Convert.ChangeType(value, to);
        }
    }

    #endregion

    #region UI_BUTTONS_DATA
    public UiMonoWithMethods GenerateButtonInformation(MainMonoWithName mono, GameObject incTarget = null)
    {
        GameObject target = null;

        if (incTarget != null)
        {
            target = incTarget;
        }
        else
        {
            target = this.Target;
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
        };

        return result;
    }
    #endregion

    #region TESTS
    private void GenerateTargetsTest()
    {
        var positions = new Vector3[] {
            new Vector3(-5, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(5, 0, 0),
        };

        for (int i = 0; i < 3; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = positions[i];
            var scr = obj.AddComponent<TargetBehaviour>();
            scr.SetUp(i);
            this.targets.Add(obj);
        }
    }
    #endregion

    #region PRUNE_DESTROYED_MONOBEHAVIOURS
    private void PruneDestroyedMonoBehaviurs()
    {
        var gameObjectsPruned = new Dictionary<GameObject, List<string>>();

        foreach (var item in this.attachedMonos)
        {
            var monoList = item.Value;

            for (int i = monoList.Count - 1; i >= 0; i--)
            {
                var mono = monoList[i];
                if (mono.Mono == null)
                {
                    if (!gameObjectsPruned.ContainsKey(item.Key))
                    {
                        gameObjectsPruned[item.Key] = new List<string>();
                    }

                    gameObjectsPruned[item.Key].Add(monoList[i].Name);

                    Debug.Log("Mono Has Been Destoryed!");
                    monoList.RemoveAt(i);
                }
            }
        }

        foreach (var item in gameObjectsPruned)
        {
            var obj = item.Key;
            foreach (var str in item.Value)
            {
                this.manageButtons.RemoveDestroyedMono(obj, str);
            }
        }
    }
    #endregion

    #region END_BRAKET
}
#endregion





#region NOT_IN_USE
//private void ManualCheckingForSingleInputValues()
//{
////INT
//if(currentParamType == typeof(int))
//{
//    var success = int.TryParse(value, out int val);
//    if (!success)
//    {
//        Debug.Log("failed at converting int!");
//        return null;
//    }
//    result.Add(val);
//}

////FLOAT
//if (currentParamType == typeof(float))
//{
//    var success = float.TryParse(value, out float val);
//    if (!success)
//    {
//        Debug.Log("failed at converting float!");
//        return null;
//    }
//    result.Add(val);
//}

////SHORT
//if (currentParamType == typeof(long))
//{
//    var success = short.TryParse(value, out short val);
//    if (!success)
//    {
//        Debug.Log("failed at converting Short!");
//        return null;
//    }
//    result.Add(val);
//}

////LONG
//if (currentParamType == typeof(long))
//{
//    var success = long.TryParse(value, out long val);
//    if (!success)
//    {
//        Debug.Log("failed at converting long!");
//        return null;
//    }
//    result.Add(val);
//}
//}
#endregion

