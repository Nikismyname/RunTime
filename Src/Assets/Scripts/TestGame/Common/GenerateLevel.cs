#region INIT

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

    #endregion

    #region CYLINDER BASE PREFAB

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

    /// <summary>
    /// collider = true; scale = new Vector3(40, 1, 40)
    /// </summary>
    /// <returns></returns>
    public GameObject CylinderBasePrefabStand1()
    {
        var prefab = this.ms.cylinderPrefab;
        var baseCilinder = GameObject.Instantiate(prefab);

        baseCilinder.name = "BaseCilinder";
        if (true == true)
        {
            var currentCollider = baseCilinder.AddComponent<MeshCollider>();
            currentCollider.convex = true;
        }
        baseCilinder.GetComponent<Renderer>().material.color = Colors.BaseColor;
        baseCilinder.transform.localScale = new Vector3(40, 1, 40);
        baseCilinder.transform.position = new Vector3(0, -new Vector3(40, 1, 40).y, 0);
        return baseCilinder;
    }

    #endregion

    #region ENTITY 

    public GameObject GenerateEntity(
        EntityType enType,
        Vector3 postions,
        PrimitiveType pType = PrimitiveType.Sphere,
        Color? color = null,
        Vector3? scale = null,
        string GOName = "entity",
        Type[] scriptTypes = null)
    {
        if (scriptTypes == null)
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
            this.ms.RegisterTarget(entity, TargetType.Standard);
        }

        if(enType == EntityType.Context)
        {
            entity.AddComponent<ContextBehaviour>();
        }

        entity.transform.localScale = scale.Value;

        var y = entity.GetComponent<MeshRenderer>().bounds.size.y;
        entity.transform.position = postions + new Vector3(0, y / 2, 0);

        entity.GetComponent<Renderer>().material.color = color.Value;
        entity.name = GOName;

        return entity;
    }

    public Script AddEditableScriptToEntity<Script>(GameObject entity, string source) where Script: MonoBehaviour
    {
        Script script = entity.AddComponent<Script>();
        var funcs = Compilation.GenerateAllMethodsFromMonoType(script.GetType());
        ms.AttachCompiletimeMono(entity, funcs, script, source);
        return script;
    }

    public GameObject GenerateTestEntity(
        string testName,
        string GOName,
        TargetType tType,
        Vector3? position = null,
        Vector3? scale = null,
        PrimitiveType pType = PrimitiveType.Sphere
        )
    {
        if (position == null) { position = Vector3.zero; }
        if (scale == null) { scale = new Vector3(1, 1, 1); }

        var testEntity = GameObject.CreatePrimitive(pType);
        var tb = testEntity.AddComponent<TargetBehaviour>();
        ms.RegisterTarget(testEntity, tType, testName);
        testEntity.name = GOName;
        testEntity.transform.position = position.Value;
        testEntity.transform.localScale = scale.Value;

        return testEntity;
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
            this.ms.RegisterTarget(entity, TargetType.Standard);

            if (scriptsToRegister != null)
            {
                foreach (var scriptType in scriptsToRegister)
                {
                    var script = (MonoBehaviour)entity.GetComponent(scriptType);
                    var funcs = Compilation.GenerateAllMethodsFromMonoType(scriptType);
                    this.ms.AttachCompiletimeMono(entity, funcs, script, "");
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

    #endregion

    #region SPACE SHIP

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
            this.ms.RegisterTarget(scriptsObject, TargetType.Standard);

            if (scriptsToRegister != null)
            {
                foreach (var scriptType in scriptsToRegister)
                {
                    var script = (MonoBehaviour)scriptsObject.GetComponent(scriptType);
                    var funcs = Compilation.GenerateAllMethodsFromMonoType(scriptType);
                    this.ms.AttachCompiletimeMono(scriptsObject, funcs, script, "");
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

    #endregion

    #region PLAYER

    public GameObject Player(Vector3 position, bool kinematic, bool rigidBody, bool isTarget, params Type[] scripts)
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

        PlayerHandling2 ph = player.AddComponent<PlayerHandling2>();
        this.rb.RegisterPlayerHandling(ph);

        if (isTarget)
        {
            var tb = player.AddComponent<TargetBehaviour>();
            //tb.SetUp(99, TargetType.Standard);
            this.ms.RegisterTarget(player, TargetType.Standard);
        }

        if (rigidBody == false)
        {
            GameObject.Destroy(player.GetComponent<Rigidbody>());
        }

        position.y += 1;

        player.transform.position = position;

        ReferenceBuffer.Instance.PlayerObject = player;

        return player;
    }

    public GameObject PlayerWithCamStand1()
    {
        GameObject player;
        Vector3 position = new Vector3(0, 0, 10);

        player = GameObject.Instantiate(this.ms.playerKinematicPrefab);

        player.name = "Player";
        player.AddComponent<PlayerFailure>();

        var ph = player.AddComponent<PlayerHandling2>();
        this.rb.RegisterPlayerHandling(ph);

        var tb = player.AddComponent<TargetBehaviour>();
        //tb.SetUp(99, TargetType.Standard);
        this.ms.RegisterTarget(player, TargetType.Standard);

        position.y += 1;
        player.transform.position = position;

        GameObject mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = mainCamera.GetComponent<CamHandling>();
        camHandling.target = player.transform;

        ReferenceBuffer.Instance.PlayerObject = player;

        return player;
    }

    #endregion

    #region GENERATE GRID

    /// <summary>
    /// Generates a grid based level platform
    /// </summary>
    /// <param name="X">The number of columns</param>
    /// <param name="Y">The number of raws</param>
    /// <param name="position">The end position of the level</param>
    /// <returns>The grid parent GameObject</returns>
    public GameObject GenerateGrid(int Y, int X, Vector3? position = null)
    {
        if (position == null)
        {
            position = Vector3.zero;
        }

        var sideLenght = 5f;
        var margin = sideLenght * 0.1f;

        var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var planeX = (X + 2) * sideLenght + (X + 3) * margin;
        var planeY = (Y + 2) * sideLenght + (Y + 3) * margin;
        plane.transform.localScale = new Vector3(planeX, 1, planeY);
        plane.transform.position = new Vector3(planeX / 2 /*-( margin + sideLenght)*/, -0.5f, planeY / 2 /*- (margin + sideLenght)*/);
        plane.GetComponent<MeshRenderer>().material.color = Color.blue;
        plane.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Color");
        plane.name = "BasePlane";

        var oriantationTyle = CreateTile(0, 0, false);

        oriantationTyle.GetComponent<MeshRenderer>().material.color = Colors.OrientationTile;

        for (int i = 1; i <= Y; i++)
        {
            for (int j = 1; j <= X; j++)
            {
                CreateTile(i, j, true);
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
                if (this.gm != null)
                {
                    this.gm.RegisterTyle(y - 1, x - 1, script);
                }
                else
                {
                    Debug.Log("Grid Manager Not Present!");
                }

                script.SetUp(y - 1, x - 1);
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

    #endregion

    #region

    public GameObject CreatePlottedWall
        (
            Vector3 position,
            Vector3 scale,
            Vector3 rotation,
            Color color,
            bool plot,
            Color plotColor,
            float plotSize = 1
        )
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.localScale = scale;

        var xRepetitions = scale.x / plotSize;
        var xReminder = xRepetitions % 1;
        var yRepetitions = scale.y / plotSize;
        var yReminder = yRepetitions % 1;

        if (xReminder > 0.5)
        {
            xRepetitions = (float)Math.Floor(xRepetitions) + 1;
        }
        else
        {

            xRepetitions = (float)Math.Floor(xRepetitions);
        }

        if (yReminder > 0.5)
        {
            yRepetitions = (float)Math.Floor(yRepetitions) + 1;
        }
        else
        {

            yRepetitions = (float)Math.Floor(yRepetitions);
        }

        var xLength = scale.x / xRepetitions;
        var yLength = scale.y / yRepetitions;

        xRepetitions += 1;
        yRepetitions += 1;

        for (int i = 0; i < xRepetitions; i++)
        {
            var xPos = i * xLength - (scale.x / 2);
            var frontLine = this.CreateLine
            (
                new Vector3(xPos, 0, -scale.z / 2),
                new Vector3(0, 0, 0),
                new Vector3(0.1f, scale.y / 2, 0.1f),
                Color.red,
                wall.transform
            );
            frontLine.name = "Front 1";

            var backLine = this.CreateLine
            (
                new Vector3(xPos, 0, scale.z / 2),
                new Vector3(0, 0, 0),
                new Vector3(0.1f, scale.y / 2, 0.1f),
                Color.red,
                wall.transform
            );
            backLine.name = "Back 1";
        }

        for (int i = 0; i < yRepetitions; i++)
        {
            var yPos = i * yLength - (scale.y / 2);
            var frontLine = this.CreateLine
            (
                new Vector3(0, yPos, -scale.z / 2),
                new Vector3(0, 0, 90),
                new Vector3(0.1f, scale.y / 2, 0.1f),
                Color.red,
                wall.transform
            );
            frontLine.name = "Front 2";

            var backLine = this.CreateLine
            (
                new Vector3(0, yPos, scale.z / 2),
                new Vector3(0, 0, 90),
                new Vector3(0.1f, scale.y / 2, 0.1f),
                Color.red,
                wall.transform
            );
            backLine.name = "Back 2";
        }

        wall.transform.position = position;
        wall.transform.eulerAngles = rotation;

        return wall;
    }

    private GameObject CreateLine
        (
            Vector3 position,
            Vector3 rotation,
            Vector3 scale,
            Color color,
            Transform parent
        )
    {
        var line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        GameObject.Destroy(line.GetComponent<Collider>());
        line.GetComponent<Renderer>().material.color = color;
        line.transform.localScale = scale;
        line.transform.position = position;
        line.transform.eulerAngles = rotation;
        line.transform.parent = parent;
        return line;
    }

    #endregion
}

