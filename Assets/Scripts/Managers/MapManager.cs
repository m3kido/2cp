using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    // Access to different tilemaps
    [SerializeField] private Tilemap _map;
    [SerializeField] private Tilemap _highlightMap;
    [SerializeField] private Tilemap _borderMap;
    [SerializeField] private Tilemap _arrowMap;

    [SerializeField] private List<TerrainDataSO> _tileDatas;
    [SerializeField] private AnimatedTile _highlightedTile;
    [SerializeField] private RuleTile _borderedTile;
    [SerializeField] private Tile[] _arrowTiles;

    // Dictionary mapping a tile to its terrain data
    private Dictionary<Tile, TerrainDataSO> _dataFromTile = new();

    // Readonly properties for the previous fields
    public Tilemap Map => _map;
    public Tilemap HighlightMap => _highlightMap;
    public Tilemap Bordermap => _borderMap;
    public Tilemap ArrowMap => _arrowMap;
    public List<TerrainDataSO> TileDatas => _tileDatas;
    public AnimatedTile HighlightedTile => _highlightedTile;
    public RuleTile BorderedTiles => _borderedTile;
    public Tile[] ArrowTiles => _arrowTiles;
    public Dictionary<Tile, TerrainDataSO> DataFromTile => _dataFromTile;

    // Get tile datas of every tile type from the inspector
    private void Awake()
    {
        
        foreach (var tileData in _tileDatas)
        {
            foreach(var tile in tileData.TerrainsOfSameType) 
            {
                _dataFromTile.Add(tile, tileData);
            }
        }    
    }

    // Get data of given tile
    public TerrainDataSO GetTileData(Tile tile)
    {
        return _dataFromTile[tile];
    }

    // Get data of given grid position - Overloading GetTileData()
    public TerrainDataSO GetTileData(Vector3Int Pos)
    {
        var tile = _map.GetTile<Tile>(Pos);
        if (tile == null) { return null; }
        return _dataFromTile[tile];

    }

    // Highlight the given grid position
    public void HighlightTile(Vector3Int pos)
    {
        _highlightMap.SetTile(pos, _highlightedTile);
        _borderMap.SetTile(pos, _borderedTile);
        _borderMap.SetColor(pos, Color.yellow);
    }

    // Unhighlight the given grid position
    public void UnHighlightTile(Vector3Int pos)
    {
        _highlightMap.SetTile(pos, null);
        _borderMap.SetTile(pos, null);
      
    }

    // Select the adequate arrow sprite based on the next tile and the previous one
    public void DrawArrow(Vector3Int prev, Vector3Int curr, Vector3Int next)
    {
        EArrowDirections arrow = EArrowDirections.None;

        Vector2 distancePrev = new(curr.x - prev.x, curr.y - prev.y);
        Vector2 distanceNext = new(next.x - curr.x, next.y - curr.y);
        Vector2 distance = distancePrev != distanceNext ? distanceNext + distancePrev : distanceNext;

        if(distance == new Vector2(0, 0))
        {
            arrow = EArrowDirections.None ;
        }
        else if (distance == new Vector2(1, 0) && next == curr )
        {
            arrow = EArrowDirections.Right;
        }
        else if (distance == new Vector2(-1, 0) && next == curr)
        {
            arrow = EArrowDirections.Left;
        }
        else if (distance == new Vector2(0, 1) && next == curr)
        {
            arrow = EArrowDirections.Up;
        }
        else if (distance == new Vector2(0, -1) && next == curr)
        {
            arrow = EArrowDirections.Down;
        }
        else if (distance == new Vector2(1, 0) || distance == new Vector2(-1, 0))
        {
            arrow = EArrowDirections.Horizontal;
        }
        else if (distance == new Vector2(0, 1) || distance == new Vector2(0, -1))
        {
            arrow = EArrowDirections.Vertical;
        }
        else if (distance == new Vector2(1, 1))
        {
            if (curr.y != prev.y)
            {
                arrow = EArrowDirections.TopRight;
            }
            else
            {
                arrow = EArrowDirections.BottomLeft;
            }
        }
        else if (distance == new Vector2(-1, 1))
        {
            
            if (curr.y != prev.y)
            {
                arrow = EArrowDirections.TopLeft;
            }
            else
            {
                arrow = EArrowDirections.BottomRight;
            }

        }
        else if (distance == new Vector2(1, -1))
        {
           
            if (curr.y != prev.y)
            {
                arrow = EArrowDirections.BottomRight;
            }
            else
            {
                arrow = EArrowDirections.TopLeft;
            }

        }
        else if (distance == new Vector2(-1, -1))
        {
            
            if (curr.y != prev.y)
            {
                arrow = EArrowDirections.BottomLeft;
            }
            else
            {
                arrow = EArrowDirections.TopRight;
            }
        }
        _arrowMap.SetTile(curr, _arrowTiles[(int)arrow]);
    }
}