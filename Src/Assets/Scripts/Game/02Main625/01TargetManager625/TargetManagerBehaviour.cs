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
///     Comunicating current target and attached monos to the UI <see cref="GenerateUpdateAndDisplayTargetSpeceficUI">
/// </summary>
public class TargetManagerBehaviour : MonoBehaviour
{
    /// Reference to the script that manages action buttons 
    private GenerateUpdateAndDisplayTargetSpeceficUI manageProcUI;

    public TargetRegistry registry;

    public TargetSelector selector;

    public TargetAttacher attacher;

    void Awake()
    {
        this.manageProcUI = ReferenceBuffer.Instance.ManageProcUI;
        ///Every two seconds check for destroyed monos and send message to <see cref="GenerateUpdateAndDisplayTargetSpeceficUI"> to remove them from the UI. 
        InvokeRepeating("PruneDestroyedMonoBehaviurs", 1, 2);

        this.registry = new TargetRegistry();
        this.selector = new TargetSelector(this.registry.Targets);
        this.attacher = new TargetAttacher(selector.Target);
    }
    #endregion


    #region CALL_FUNCTION
    public void CallFunction(string monoName, string methodName, ParameterNameWithSingleObjectValues[] parameters)
    {
        if (this.selector.Target == null)
        {
            Debug.Log("Select a target before calling function!");
            return;
        }

        if (!this.attacher.AttachedMonos.ContainsKey(this.selector.Target))
        {
            Debug.Log("The target has no behaviours attached!");
            return;
        }

        var monos = this.attacher.AttachedMonos[this.selector.Target];

        var wantedMonos = monos.Where(x => x.Name == monoName).ToArray();
        if (wantedMonos.Length != 1)
        {
            Debug.Log("wanted behaviour not found or more than one found");
            return;
        }

        var wantedMono = wantedMonos[0];

        var wantedMethods = wantedMono.Methods.Where(x => x.Name == methodName).ToArray();

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
            scr.SetUp(i, TargetType.Standard);
            this.registry.Targets.Add(obj);
        }
    }
    #endregion

    #region PRUNE_DESTROYED_MONOBEHAVIOURS
    private void PruneDestroyedMonoBehaviurs()
    {
        var gameObjectsPruned = new Dictionary<GameObject, List<string>>();

        foreach (var item in this.attacher.AttachedMonos)
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
                this.manageProcUI.RemoveDestroyedMono(obj, str);
            }
        }
    }
    #endregion

    #region }
}
#endregion

