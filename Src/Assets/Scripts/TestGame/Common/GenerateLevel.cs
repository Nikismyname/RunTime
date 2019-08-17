using System;
using UnityEngine;

public class GenerateLevel
{
    private Main ms;
    private GridManager gm; 
    private ReferenceBuffer rb;

    public GenerateLevel(Main ms, ReferenceBuffer rb, GridManager gm = null)
    {
        this.ms = ms;
        this.rb = rb;
        this.gm = gm;
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
                    this.ms.RegisterCompileTimeMono(entity,funcs, script); 
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
                    this.ms.RegisterCompileTimeMono(scriptsObject, funcs, script);
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

    /// <summary>
    /// Generates a grid based level platform
    /// </summary>
    /// <param name="X">The number of columns</param>
    /// <param name="Y">The number of raws</param>
    /// <param name="position">The end position of the level</param>
    /// <returns>The grid parent GameObject</returns>
    public GameObject GenerateGrid(int Y, int X, Vector3? position = null)
    {
        if(position == null)
        {
            position = Vector3.zero; 
        }
        
        var sideLenght = 5f;
        var margin = sideLenght * 0.1f;

        var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var planeX = (X + 2) * sideLenght + (X + 3) * margin;
        var planeY = (Y + 2) * sideLenght + (Y + 3) * margin; 
        plane.transform.localScale = new Vector3(planeX, 1, planeY);
        plane.transform.position = new Vector3(planeX/2 /*-( margin + sideLenght)*/, -0.5f, planeY/2 /*- (margin + sideLenght)*/); 
        plane.GetComponent<MeshRenderer>().material.color = Color.blue;
        plane.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Color");
        plane.name = "BasePlane";

        var oriantationTyle = CreateTile(0, 0, false);

        oriantationTyle.GetComponent<MeshRenderer>().material.color = Colors.OrientationTile; 

        for (int i = 1; i <= Y; i++)
        {
            for (int j = 1; j <= X; j++)
            {
                CreateTile(i,j, true);
            }
        }

        plane.transform.position = position.Value;
        return plane;

        /// Creates a tile in the 0-based grid. 
        GameObject CreateTile(int y, int x, bool stepable)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.AddComponent<BoxCollider>();
            if (stepable)
            {
                var script = tile.AddComponent<TileBehaviour>();
                if(this.gm != null)
                {
                    this.gm.RegisterTyle(y-1,x-1,script);
                }
                else
                {
                    Debug.Log("Grid Manager Not Present!");
                }

                script.SetUp(y -1,x -1);
            }
            tile.transform.localScale = new Vector3(sideLenght, 0, sideLenght);
            tile.GetComponent<MeshRenderer>().material.color = Color.green;
            tile.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Color");
            tile.transform.position = new Vector3(
                ///+ 0.5f * sideLenght on x and z is to center tile after calculation upper left corner value
                (x + 1) * margin + (x) * sideLenght + 0.5f * sideLenght,
                0.01f,
                (y + 1) * margin + (y) * sideLenght + 0.5f * sideLenght 
            );

            tile.transform.SetParent(plane.transform);

            return tile;
        }
    }
}
