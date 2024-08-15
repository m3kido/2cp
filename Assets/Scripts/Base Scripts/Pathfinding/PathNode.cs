using UnityEngine;

public class PathNode 
{
    public int gcost;
    public int hcost;
    public int fcost;

    public Vector3Int pos;
    public PathNode lastpos;

    public PathNode(Vector3Int pos, PathNode lastpos, int currgcost, int currhcost)
    {
        this.lastpos = lastpos;
        this.pos = pos;
        gcost = currgcost;
        hcost = currhcost;
        fcost = currhcost + currgcost;
    }
}
