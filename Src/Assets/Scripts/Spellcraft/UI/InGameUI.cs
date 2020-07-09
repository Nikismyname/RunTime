using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    private Camera myCamera;
    private SpellcraftCam camHanding;
    private Node dragged = null;
    LineDrawer drawer;
    Defunclator classVisualisation;
    GameObject text1;
    TMP_Text worldSpaceText;
    GameObject menu1;

    private void Start()
    {
        this.text1 = GameObject.Find("WSCText1");
        this.menu1 = GameObject.Find("WSCMenu1");
        //this.worldSpaceText = this.worldSpaceCanvas.transform.Find("Text").GetComponent<Text>();
        this.worldSpaceText = this.text1.transform.Find("Text").GetComponent<TMP_Text>();

        this.DrawBox(SpellcraftConstants.HalfSize, SpellcraftConstants.Thickness, SpellcraftConstants.BoxCenter);
        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();
        this.camHanding = this.myCamera.gameObject.AddComponent<SpellcraftCam>();
        this.camHanding.SetTarget(new GameObject("Center"));
        this.drawer = gameObject.GetComponent<LineDrawer>();
        this.classVisualisation = new Defunclator(this);

        //ClassTest();
        this.classVisualisation.GenAll(this.classVisualisation.GenerateNodeData<Some>());
    }

    private PropertyNode lastClicked = null; 

    public void RegisterPropClick(PropertyNode node)
    {
        if(this.lastClicked == null)
        {
            this.lastClicked = node;
        }
        else
        {
            this.drawer.RegisterCurve(this.lastClicked.gameObject.transform, node.gameObject.transform, node.gameObject.transform.parent.transform.position, 0.1f, Color.cyan, 1);
            this.lastClicked = null;
        }
    }

    public void SetWorldCanvasText(string text)
    {
        this.worldSpaceText.text = text; 
    }

    public void SetWorldCanvasPosition(Vector3 position)
    {
        RectTransform rt = this.text1.GetComponent<RectTransform>();
        rt.position = position;
        rt.LookAt(rt.transform.position + this.myCamera.transform.forward);
    }

    private void ClassTest()
    {
        this.classVisualisation.BuildUI(this.classVisualisation.GenerateNodeData<Some>());
    }

    public class Some
    {
        public int one { get; set; }

        public string  two { get; set; }

        public int three { get; set; }

        public string four { get; set; }

        public int SomeOne(int one, int two , int three, int four)
        {
            return 0;
        }

        public int SomeTwo(int one, int two, int three, int four)
        {
            return 0;
        }

        public int SomeThree(int one, int two, int three, int four)
        {
            return 0;
        }

        public int SomeFour(int one, int two, int three, int four, int five)
        {
            return 0;
        }
    }

    private void DefaultInterface()
    {
        GameObject node = GameObject.CreatePrimitive(PrimitiveType.Cube);
        node.AddComponent<Node>().Setup(this);
        node.name = "node";
        GameObject node2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        node2.AddComponent<Node>().Setup(this);
        node2.name = "node";
        this.drawer.RegisterLine(node.transform, node2.transform, 0.2f, Color.cyan);
    }

    public void SetDragged(Node dragged)
    {
        this.dragged = dragged;
    }

    private GameObject DrawInGameLine(Vector3 from, Vector3 to, Color color, float thickness)
    {
        GameObject parent = new GameObject("LineParent");
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        parent.transform.position = new Vector3(0, -1, 0);
        line.transform.parent = parent.transform;

        parent.transform.position = from;
        parent.transform.LookAt(to);
        parent.transform.Rotate(new Vector3(1, 0, 0), 90);
        line.GetComponent<Renderer>().material.color = color;
        line.SetShader();
        parent.SetScale(new Vector3(thickness, (from - to).magnitude / 2, thickness));
        return parent;
    }

    private void DrawBox(float halfSize, float thickness, Vector3 center)
    {
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, -halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness);
        this.DrawInGameLine(center + new Vector3(-halfSize, halfSize, -halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            this.dragged?.SetRotating(true);
            this.camHanding.GetMouseButtonDownOne();
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.camHanding.GetMouseButtonUpOne();

            if (this.dragged != null)
            {
                Vector3 thing = this.myCamera.WorldToScreenPoint(dragged.gameObject.transform.position);

            }

            this.dragged?.SetRotating(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.camHanding.UntriggerZoom();
        }
    }
}

