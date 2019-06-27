using System;
using UnityEngine;

public class GenerateLevel
{
    private Main ms;

    public GenerateLevel(Main ms)
    {
        this.ms = ms;
    }

    public GameObject CylinderBasePrefab(bool collider = false)
    {
        var prefab = this.ms.cylinderPrefab;
        var baseCilinder = GameObject.Instantiate(prefab);

        float yScale = 1;
        baseCilinder.name = "BaseCilinder";
        if (collider == true)
        {
            var currentCollider = baseCilinder.AddComponent<MeshCollider>();
            currentCollider.convex = true;
        }
        baseCilinder.GetComponent<Renderer>().material.color = Color.blue;
        baseCilinder.transform.localScale = new Vector3(30, yScale, 30);
        baseCilinder.transform.position = new Vector3(0, -yScale, 0);
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

    public GameObject Player(Vector3 position, bool rigidBody,bool isTarget, params Type[] scripts)
    {
        var player = GameObject.Instantiate(this.ms.playerPrefab);
        player.name = "Player";

        player.AddComponent<PlayerScript>();

        if (isTarget)
        {
            var tb = player.AddComponent<TargetBehaviour>();
            tb.SetUp(99);
            this.ms.RegisterTarget(player);
        }

        if (rigidBody)
        {
            var rb = player.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.freezeRotation = true;
        }
        position.y += 1; 

        player.transform.position = position;
        return player;
    }
}
