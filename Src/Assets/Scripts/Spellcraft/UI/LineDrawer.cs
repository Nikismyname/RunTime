#region INIT

using System;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer
{
    private List<Line> lines = new List<Line>();
    private WorldSpaceUI UI;

    public LineDrawer(WorldSpaceUI UI)
    {
        this.UI = UI;
    }

    #endregion

    #region UPDATE

    private void Update()
    {
        foreach (var line in this.lines)
        {
            line.Update();
        }
    }

    #endregion

    #region DRAW_DYNAMIC

    public void DrawDynamicLine(Transform transOne, Transform transTwo, float thickness, Color color)
    {
        this.lines.Add(new Line(transOne, transTwo, thickness, color, this.UI.levelSpecificParent));
    }

    /// <summary>
    /// Paranted to the class node sphere, no need to global parent them, need to global parent the spheres.
    /// </summary>
    public void DrawDynamicCurve(Transform transOne, Transform transTwo, Vector3 center, float scale, Color color, int level, Transform parent = null, float radius = 0.5f)
    {
        parent = parent == null ? transOne.parent.transform : parent;

        Vector3 one = transOne.position - center;
        Vector3 two = transTwo.position - center;

        one = one * 1.2f;
        two = two * 1.2f;

        float angleFromCenter = Vector3.Angle(one, two);
        float distance = 2 * Mathf.PI * radius * (angleFromCenter / 360);

        int count = (int)Math.Floor(distance / 0.1f);
        if (count < 2)
        {
            count = 2;
        }

        Vector3? previousPosition = null;

        for (int i = 0; i < count + 1; i++)
        {
            Vector3 newPos = Vector3.Slerp(one, two, (float)(i) / count);
            //GameObject sphere = this.CreateCurveSphere(newPos, Color.cyan, 0.1f, parent);

            if (previousPosition != null)
            {
                this.CreateCurveCylinder(newPos, previousPosition.Value, color, scale, parent);
            }

            previousPosition = newPos;
        }
    }

    //private GameObject CreateCurveSphere(Vector3 pos, Color color, float scale, Transform parent)
    //{
    //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    go.GetComponent<Renderer>().material = this.UI.transperantMat;
    //    Color col = new Color(color.r, color.g, color.g, 0.0f);
    //    go.SetColor(col);
    //    go.transform.position = pos;
    //    go.SetScale(new Vector3(scale, scale, scale));
    //    go.SetActive(false);
    //    go.transform.parent = parent;

    //    return go;
    //}

    private GameObject CreateCurveCylinder(Vector3 one, Vector3 two, Color color, float scale, Transform parent)
    {
        var cylinderParent = new GameObject("LineParent");
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinderParent.transform.position = new Vector3(0, -1, 0);
        line.transform.parent = cylinderParent.transform;
        cylinderParent.transform.position = one;
        cylinderParent.transform.LookAt(two);
        cylinderParent.transform.Rotate(new Vector3(1, 0, 0), 90);
        line.GetComponent<Renderer>().material.color = color;
        line.GetComponent<Renderer>().material = this.UI.transperantMat;
        Color col = new Color(color.r, color.g, color.g, 0.2f);
        line.SetColor(col);
        cylinderParent.SetScale(new Vector3(scale, (one - two).magnitude / 2, scale));
        cylinderParent.transform.parent = parent.transform;
        return null;
    }

    #endregion

    #region DRAW_STATIC

    public void DrawBox(float halfSize, float thickness, Vector3 center)
    {
        Transform parent = new GameObject("CubeParent").transform;
        parent.SetParent(this.UI.levelSpecificParent.transform);

        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, -halfSize, halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(-halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(halfSize, halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness, parent);
        this.DrawInGameLine(center + new Vector3(-halfSize, halfSize, -halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness, parent);
    }

    private GameObject DrawInGameLine(Vector3 from, Vector3 to, Color color, float thickness, Transform parent)
    {
        GameObject localParent = new GameObject("LineParent");
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        localParent.transform.position = new Vector3(0, -1, 0);
        line.transform.parent = localParent.transform;

        localParent.transform.position = from;
        localParent.transform.LookAt(to);
        localParent.transform.Rotate(new Vector3(1, 0, 0), 90);
        line.GetComponent<Renderer>().material.color = color;
        line.SetShader();
        localParent.SetScale(new Vector3(thickness, (from - to).magnitude / 2, thickness));
        localParent.transform.SetParent(parent);
        return localParent;
    }

    #endregion

    #region DATA CLASSES

    private class Line
    {
        private Transform transOne;
        private Vector3 transOneLast;
        private Transform transTwo;
        private Vector3 transTwoLast;
        private GameObject parent;
        private float thickness;
        private bool shouldTrack = true;

        public Line(Transform transOne, Transform transTwo, float thickness, Color color, GameObject externalParent)
        {
            this.transOne = transOne;
            this.transTwo = transTwo;
            this.thickness = thickness;
            this.CreateLine(color, externalParent);

            this.transOneLast = transOne.position;
            this.transTwoLast = transTwo.position;
        }

        private void CreateLine(Color color, GameObject externalParent)
        {
            this.parent = new GameObject("LineParent");
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            this.parent.transform.position = new Vector3(0, -1, 0);
            line.transform.parent = parent.transform;

            this.parent.transform.position = this.transOne.position;
            this.parent.transform.LookAt(this.transTwo.position);
            this.parent.transform.Rotate(new Vector3(1, 0, 0), 90);
            line.GetComponent<Renderer>().material.color = color;
            line.SetShader();
            this.parent.SetScale(new Vector3(this.thickness, (this.transOne.position - this.transTwo.position).magnitude / 2, this.thickness));
            this.parent.transform.SetParent(externalParent.transform);
        }

        public void Update()
        {
            if ((transOne.position != transOneLast || transTwo.position != transTwoLast) && this.shouldTrack)
            {
                this.parent.transform.position = this.transOne.position;
                this.parent.transform.LookAt(this.transTwo.position);
                this.parent.transform.Rotate(new Vector3(1, 0, 0), 90);
                this.parent.SetScale(new Vector3(this.thickness, (this.transOne.position - this.transTwo.position).magnitude / 2, this.thickness));

                this.transOneLast = this.transOne.position;
                this.transTwoLast = this.transTwo.position;
            }
        }

        public void Disable()
        {
            this.parent.SetActive(false);
        }

        public void Enable()
        {
            this.parent.SetActive(true);
        }

        public void StopTracking()
        {
            this.shouldTrack = true;
        }

        public void StartTracking()
        {
            this.shouldTrack = false;
        }
    }

    #endregion
}