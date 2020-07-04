using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Attached to the main of levels with grids. 
/// Keeps reference to all the grids and displays selections from Test solutions.
/// </summary>
public class GridManager : MonoBehaviour
{
    const float MinSelectionSpan = 0.1f;
    const float SpanTarget = 2f; 

    private Dictionary<Point, TileBehaviour> tiles = new Dictionary<Point, TileBehaviour>(); 

    public void RegisterTyle(int y, int x, TileBehaviour tb)
    {
        var point = new Point(y, x);
        if (this.tiles.ContainsKey(point))
        {
            Debug.Log("Tile already registerd!");
            Debug.Break();
            return;
        }
        this.tiles.Add(point, tb);
    }

    public async Task LightTiles(int [][] selections)
    {
        var numberOfSections = selections.Length;
        var spanTime = SpanTarget / (float)numberOfSections; 
        if(spanTime < MinSelectionSpan)
        {
            spanTime = MinSelectionSpan;
        }

        for (int i = 0; i < selections.Length; i++)
        {
            for (int j = 0; j < selections[i].Length; j += 2)
            {
                var y = selections[i][j];
                var x = selections[i][j + 1];
                var point = new Point(y, x);
                if (!this.tiles.ContainsKey(point))
                {
                    Debug.Log("Point Not Found!");
                    return;
                }
                var tile = this.tiles[point];
                tile.Select();
            }
            if (i != selections.Length - 1)
            {
                await Task.Delay((int) Math.Round(spanTime * 1000f));
            }
        }

        await Task.Delay(1000);
        return; 
    } 
}

public class Point: IEquatable<Point>
{
    public int Y { get; set; }
    public int X { get; set; }

    public Point(int y, int x)
    {
        this.Y = y;
        this.X = x; 
    }

    //public bool Equals(Point x, Point y)
    //{
    //    if(x.X == y.X && x.Y == y.Y)
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    public override int GetHashCode()
    {
        return $"{this.Y}|{this.X}".GetHashCode();
    }

    //public int GetHashCode(Point obj)
    //{
    //    return $"{obj.Y}|{obj.X}".GetHashCode();
    //}

    public bool Equals(Point other)
    {
        if (this.X == other.X && this.Y == other.Y)
        {
            return true;
        }

        return false;
    }
}

