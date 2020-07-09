﻿using Boo.Lang;
using System;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private List<Line> lines = new List<Line>();

    public void RegisterLine(Transform transOne, Transform transTwo, float thickness, Color color)
    {
        this.lines.Add(new Line(transOne, transTwo, thickness, color));
    }

    public void RegisterCurve(Transform transOne, Transform transTwo, Vector3 center, float scale, Color color, int level, float radius = 0.5f)
    {
        Vector3 one = transOne.position - center;
        Vector3 two = transTwo.position - center;

        one = one * 1.2f;
        two = two * 1.2f;

        float angleFromCenter = Vector3.Angle(one, two);
        float distance = 2 * Mathf.PI * radius * (angleFromCenter / 360);

        int count = (int)Math.Floor(distance / 0.1f);

        GameObject previousSphere = null;

        for (int i = 0; i < count + 1; i++)
        {
            Vector3 newPos = Vector3.Slerp(one, two, (float)(i)/count);
            GameObject sphere = this.CreateSphere(newPos, Color.cyan, 0.1f);

            if(previousSphere != null)
            {
                this.CreateCurveCylinder(sphere.transform.position, previousSphere.transform.position, color, scale);
            }

            previousSphere = sphere;
        } 
    } 

    private GameObject CreateSphere(Vector3 pos, Color color, float scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.SetColor(color);
        go.SetShader();
        go.transform.position = pos;
        go.SetScale(new Vector3(scale, scale, scale));
        return go;
    }

    private GameObject CreateCurveCylinder(Vector3 one, Vector3 two, Color color, float scale)
    {
        var parent = new GameObject("LineParent");
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        parent.transform.position = new Vector3(0, -1, 0);
        line.transform.parent = parent.transform;
        parent.transform.position = one;
        parent.transform.LookAt(two);
        parent.transform.Rotate(new Vector3(1, 0, 0), 90);
        line.GetComponent<Renderer>().material.color = color;
        line.SetShader();
        parent.SetScale(new Vector3(scale,(one - two).magnitude/2, scale));
        return null;
    }

    private void Update()
    {
        foreach (var line in this.lines)
        {
            line.Update();
        }
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

        public Line(Transform transOne, Transform transTwo, float thickness, Color color)
        {
            this.transOne = transOne;
            this.transTwo = transTwo;
            this.thickness = thickness;
            this.CreateLine(color);

            this.transOneLast = transOne.position;
            this.transTwoLast = transTwo.position;
        }

        private void CreateLine(Color color)
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
        }

        public void Update()
        {
            if((transOne.position != transOneLast || transTwo.position != transTwoLast)&& this.shouldTrack)
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

