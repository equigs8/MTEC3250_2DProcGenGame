using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    private GridGenerator grid;
    public List<PathPoint> open = new List<PathPoint>();
    public List<PathPoint> closed = new List<PathPoint>();
    bool done = true;
    [HideInInspector] public PathPoint startPoint;
    [HideInInspector] public PathPoint endPoint;
    [HideInInspector] public PathPoint pathPoint;
    [HideInInspector] public PathPoint lastPathPoint;

    private void Start()
    {
        grid = GridGenerator.inst;      
    }


    public Stack<PathPoint> SearchAndGetPath(Tile start, Tile end)
    {
        done = false;

        List<Tile> locations = new List<Tile>();

        foreach (Tile t in grid.tiles)
        {
            if (!t.isInaccessible && !t.isTrap)
            {
                locations.Add(t);
            }
        }

        open.Clear();
        closed.Clear();

        startPoint = new PathPoint(start, 0, 0, 0, null);
        endPoint = new PathPoint(end, 0, 0, 0, null);

        open.Add(startPoint);
        lastPathPoint = startPoint;

        
        //lastPathPoint.tile.AdjustColor(Color.blue);

        //return;

        while (!done)
        {
            Search(lastPathPoint);
        }

        var path = GetPath();
        for (int i = 0; i < open.Count; i++)
        {
            //open.ElementAt(i).tile.AdjustColor(Color.yellow);
            //print(path.ElementAt(i).location.name + " : #" + i);

        }

        for (int i = 0; i < closed.Count; i++)
        {
            //closed.ElementAt(i).tile.AdjustColor(Color.magenta);
            //print(path.ElementAt(i).location.name + " : #" + i);

        }

        for (int i = 0; i < path.Count; i++)
        {
            //path.ElementAt(i).tile.AdjustColor(Color.cyan);
            print(path.ElementAt(i).tile.name + " : #" + i + " G:" + path.ElementAt(i).G
                                                               + " H:" + path.ElementAt(i).H
                                                               + " F:" + path.ElementAt(i).F);

        }

        return path;


    }

    public Stack<PathPoint> GetPath()
    {
        Stack<PathPoint> path = new Stack<PathPoint>();
        PathPoint point = lastPathPoint;

        while (!startPoint.Equals(point) && point != null)
        {
            path.Push(point);
            point = point.parent;

        }

        path.Push(startPoint);

        return path;
    }

    public void Search(PathPoint thisPoint)
    {
        if (thisPoint.Equals(endPoint)) { done = true; return; }

        foreach (Tile neighbor in thisPoint.tile.Neighbors())
        {
            if (IsClosed(neighbor)) continue;

            //neighbor.sr.enabled = true;

            float diag = 0;
            float dist = Vector2.Distance(thisPoint.tile.ToVector(), neighbor.ToVector());

            //if (neighbor.d != thisPoint.location.d && neighbor.w != thisPoint.location.w)
            //{

            //    diag = Mathf.Sqrt(Mathf.Pow(dist,2)+ Mathf.Pow(dist, 2));

            //}
            float G = (dist - diag) + (thisPoint.G - diag);
            float H = Vector2.Distance(neighbor.ToVector(), endPoint.tile.ToVector()) - diag;
            //float O = neighbor.Occupation;


            float F = G + H;

            if (!UpdatePoint(neighbor, G, H, F, thisPoint))
            {
                open.Add(new PathPoint(neighbor, G, H, F, thisPoint));
            }
        }

        foreach (PathPoint point in open)
        {
            print(point.tile.name + "| F: " + point.F + " - G:" + point.G + " - H:" + point.H);
        }

        List<PathPoint> openOrdered = open.OrderBy(point => point.F).ThenBy(point => point.H).ToList();
        //open.OrderBy(point => point.F).ThenBy(point => point.H).ToList();
        //open.OrderBy(p => p.F).ToList<PathPoint>();

        PathPoint pp = openOrdered.ElementAt(0);
        closed.Add(pp);
        open.Remove(pp);
        openOrdered.RemoveAt(0);

        lastPathPoint = pp;
        //print(lastPathPoint.location.name + " Selected");

        //foreach(PathPoint p in open)
        //{
        //    p.location.sr.color = Color.green;
        //}

        //foreach (PathPoint p in closed)
        //{
        //    p.location.sr.color = Color.red;
        //}

        //lastPathPoint.location.sr.color = Color.blue;


    }

    bool UpdatePoint(Tile tile, float g, float h, float f, PathPoint _parent)
    {
        foreach (PathPoint p in open)
        {
            if (p.tile.Equals(tile))
            {
                p.G = g;
                p.H = h;
                //p.O = o;
                p.F = f;
                p.parent = _parent;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(Tile tile)
    {
        foreach (PathPoint p in closed)
        {
            if (p.tile.Equals(tile)) return true;
        }
        return false;
    }

}

public class PathPoint
{
    public Tile tile;
    public PathPoint parent;
    public float G;
    public float H;
    public float F;
    //public float O;

    public PathPoint(Tile t, float g, float h, float f, PathPoint p)
    {
        tile = t;
        G = g;
        H = h;
        //O = o;
        F = f;
        parent = p;

    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !GetType().Equals(obj.GetType())) return false;
        else return tile.Equals(((PathPoint)obj).tile);
    }

    public override int GetHashCode() { return 0; }
}
