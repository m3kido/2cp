using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour 
{
    MapManager _mm;
    private void Start()
    {
         _mm = FindObjectOfType<MapManager>();
    }
    public List<Vector3Int> FindPath(Unit unit, Vector3Int start, Vector3Int end)
    {
        List<PathNode> Queue = new();
        List<PathNode> Visited = new();
        List<PathNode> possibleNodes = new();
        PathNode currNode = new(start, null, 0, Calculatehcost(start, end));

        Queue.Add(currNode);
        while (Queue.Count>0)
        {
            currNode = GetLowestfcost(Queue);
            
            if(currNode.pos == end ) { return CalculatePath(currNode); }

            Queue.Remove(currNode);
            Visited.Add(currNode);

            PathNode rightNode = ExplorePos(unit, currNode.pos + Vector3Int.right, end, Queue, Visited);
            PathNode leftNode = ExplorePos(unit, currNode.pos + Vector3Int.left, end, Queue, Visited);
            PathNode upNode = ExplorePos(unit, currNode.pos + Vector3Int.up, end, Queue, Visited);
            PathNode downNode = ExplorePos(unit, currNode.pos + Vector3Int.down, end, Queue, Visited);

            if(rightNode != null)
            {                
                possibleNodes.Add(rightNode);
            }
            if(leftNode != null)
            {
                possibleNodes.Add(leftNode);
            }
            if(upNode != null)
            {
                possibleNodes.Add(upNode);
            }
            if (downNode != null)
            {
                possibleNodes.Add(downNode);
            }
            
            foreach(var node in possibleNodes)
            {
                int newgcost = currNode.gcost + Calculategcost(unit, node.pos);
                if(newgcost < node.gcost)
                {
                    node.lastpos = currNode;
                    node.gcost = newgcost;
                    node.fcost = node.gcost+node.hcost;

                    if (!Queue.Contains(node))
                    {
                        Queue.Add(node);
                    }
                }
            }
            possibleNodes.Clear();
        }
        return null;
    }

    private PathNode ContainsPos(List<PathNode> list, Vector3Int pos)
    {
        foreach(var node in list)
        {
            if(node.pos == pos) return node;
        }
        return null;
    }

    private PathNode GetLowestfcost(List<PathNode> queue)
    {
        PathNode lowest = queue[0];
        foreach(PathNode node in queue)
        {
            if(node.fcost < lowest.fcost)
            {
                lowest = node;
            }
        }
        return lowest;
    }

    private PathNode ExplorePos(Unit unit, Vector3Int pos, Vector3Int end, List<PathNode> queue, List<PathNode> visited )
    {
        if (ContainsPos(queue, pos)!=null)
        {
            return ContainsPos(queue, pos);
        }
        if(ContainsPos(visited, pos)!=null) {
            return null;
        }
        int gcost = Calculategcost(unit, pos);
        if(gcost > 0)
        {
            PathNode newNode = new PathNode(pos, null, int.MaxValue, Calculatehcost(pos, end));
            return newNode;
        }
        else
        {
            return null;
        }
    }

    private int Calculategcost(Unit unit,Vector3Int pos)
    {
        TerrainDataSO data = _mm.GetTileData(pos);
        if (data != null)
        {
            if(unit.ValidTiles.ContainsKey(pos))
            {
                return data.ProvisionsCost;
            }
            return -1;
        }
        return -1;
    }

    private int Calculatehcost(Vector3Int start, Vector3Int end)
    {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }

    private List<Vector3Int> CalculatePath(PathNode last)
    {
        List<Vector3Int > path = new();
        PathNode curr= last;
        while (curr.lastpos != null)
        {
            path.Add(curr.pos);
            curr = curr.lastpos;
        }
        path.Reverse();
        return path ;
    }
}
