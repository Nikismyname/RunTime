using System;
using UnityEngine;

public class GenerateLevel
{
    private Main ms;
    private ReferenceBuffer rb;

    public GenerateLevel(Main ms, ReferenceBuffer rb)
    {
        this.ms = ms;
        this.rb = rb;
    }

    public GameObject CylinderBasePrefab(Vector3 scale, bool collider = false)
    {
        var prefab = this.ms.cylinderPrefab;
        var baseCilinder = GameObject.Instantiate(prefab);

        baseCilinder.name = "BaseCilinder";
        if (collider == true)
        {
            var currentCollider = baseCilinder.AddComponent<MeshCollider>();
            currentCollider.convex = true;
        }
        baseCilinder.GetComponent<Renderer>().material.color = Colors.BaseColor;
        baseCilinder.transform.localScale = scale;
        baseCilinder.transform.position = new Vector3(0, -scale.y, 0);
        return baseCilinder;
    }

    public GameObject GenerateEntity(
        EntityType enType,
        Vector3 postions,
        PrimitiveType pType = PrimitiveType.Sphere,
        Color? color = null,
        Vector3? scale = null,
        string name = "entity",
        Type[] scriptTypes = null)
    {
        if(scriptTypes == null)
        {
            scriptTypes = new Type[0];
        }

        if (scale == null)
        {
            scale = new Vector3(1, 1, 1);
        }

        if (color == null)
        {
            color = Color.gray;
        }

        var entity = GameObject.CreatePrimitive(pType);

        if (enType == EntityType.Target)
        {
            entity.AddComponent<TargetBehaviour>();
            this.ms.RegisterTarget(entity);
        }

        entity.transform.localScale = scale.Value;

        var y = entity.GetComponent<MeshRenderer>().bounds.size.y;
        entity.transform.position = postions + new Vector3(0, y / 2, 0);

        entity.GetComponent<Renderer>().material.color = color.Value; 
        entity.name = name;

        foreach (var type in scriptTypes)
        {
            entity.AddComponent(type);
        }

        return entity;
    }

    public GameObject GenerateEntityFromPrefab(
    EntityType enType,
    GameObject prefab,
    Vector3 postions,
    string bodyToWorkWith = null,
    Vector3? scale = null,
    string name = "prefab entity",
    Type[] scriptsToRegister = null)
    {
        var entity = GameObject.Instantiate(prefab);

        if (bodyToWorkWith != null)
        {
            entity = entity.transform.Find(bodyToWorkWith).gameObject;
        }

        if (enType == EntityType.Target)
        {
            entity.AddComponent<TargetBehaviour>();
            this.ms.RegisterTarget(entity);

            if(scriptsToRegister != null)
            {
                foreach (var scriptType in scriptsToRegister)
                {
                    var script = (MonoBehaviour)entity.GetComponent(scriptType);
                    var funcs = Compilation.GenerateAllMethodsFromMonoType(scriptType);
                    this.ms.AttachMono(funcs, false, entity,false,script);
                }
            }
        }

        if (scale != null)
        {
            entity.transform.localScale = scale.Value;
        }

        var y = entity.GetComponent<MeshRenderer>().bounds.size.y;
        entity.transform.position = postions;

        entity.name = name;

        return entity;
    }

    public GameObject GenerateSpaceShip(
        EntityType enType,
        GameObject prefab,
        Vector3 postions,
        string scriptsObjectName = null,
        Vector3? scale = null,
        string name = "prefab entity",
        Type[] scriptsToRegister = null)
    {
        var prefabParent = GameObject.Instantiate(prefab);

        GameObject scriptsObject = null;

        if (scriptsObjectName != null)
        {
            scriptsObject = prefabParent.transform.Find(scriptsObjectName).gameObject;
            //Debug.Log("SCRIPT OBJECT PASSED");
        }
        else
        {
            scriptsObject = prefabParent;
            //Debug.Log("NO SCRIPT OBJECT PASSED");
        }

        if (enType == EntityType.Target)
        {
            scriptsObject.AddComponent<TargetBehaviour>();
            this.ms.RegisterTarget(scriptsObject);

            if (scriptsToRegister != null)
            {
                foreach (var scriptType in scriptsToRegister)
                {
                    var script = (MonoBehaviour)scriptsObject.GetComponent(scriptType);
                    var funcs = Compilation.GenerateAllMethodsFromMonoType(scriptType);
                    this.ms.AttachMono(funcs, false, scriptsObject, false, script);
                }
            }
        }

        if (scale != null)
        {
            prefabParent.transform.localScale = scale.Value;
        }

        var y = scriptsObject.GetComponent<MeshRenderer>().bounds.size.y;
        prefabParent.transform.position = postions;

        prefabParent.name = name;

        return scriptsObject;
    }

    public GameObject Player(Vector3 position,bool kinematic, bool rigidBody,bool isTarget, params Type[] scripts)
    {
        GameObject player = null; 

        if (kinematic)
        {
            player = GameObject.Instantiate(this.ms.playerKinematicPrefab);
        }
        else
        {
            player = GameObject.Instantiate(this.ms.playerPrefab);
        }

        player.name = "Player";
        player.AddComponent<PlayerFailure>();

        if (kinematic == false)
        {
            var ph = player.AddComponent<PlayerHandling>();
            this.rb.RegisterPlayerHandling(ph);
        }
        else
        {
            var ph = player.AddComponent<PlayerHandling2>();
            //this.rb.RegisterPlayerHandling(ph);
        }

        if (isTarget)
        {
            var tb = player.AddComponent<TargetBehaviour>();
            tb.SetUp(99);
            this.ms.RegisterTarget(player);
        }

        if(rigidBody == false)
        {
            GameObject.Destroy(player.GetComponent<Rigidbody>());
        }

        position.y += 1; 

        player.transform.position = position;
        return player;
    }
}
