using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Class to represent a unit, associated to every unit prefab on the scene
public class Unit : MonoBehaviour
{
    // Managers will be needed
    protected GameManager Gm;
    protected MapManager Mm;
    protected UnitManager Um;
    protected SpriteRenderer Rend;

    [SerializeField] private UnitDataSO _data;
    public UnitDataSO Data => _data;
    public EUnits UnitType;

    // Auto-properties (the compiler automatically creates private fields for them)
    public int Health { get; set; }
    public int Provisions { get; set; }
    public bool IsSelected { get; set; }
    public bool IsMoving { get; set; }

    [SerializeField] private int _owner; // Serialization is temporary (just for tests)
    public int Owner // Property for the _hasMoved field
    {
        get => _owner;
        set => _owner = value;
    }

    private bool _hasMoved;
    public bool HasMoved // Property for the _hasMoved field 
    {
        get => _hasMoved;
        set
        {
            _hasMoved = value;
            if (_hasMoved)
            {
                Rend.color = Color.gray;
            }
            else
            {
                Rend.color = Color.white;
            }
        }
    }

    // Dictionary to hold the grid position of the valid tiles along with the provisions consumed to reach them
    private Dictionary<Vector3Int, int> _validTiles = new();
    public Dictionary<Vector3Int, int> ValidTiles
    {
        get => _validTiles;
        set => _validTiles = value;
    }

    void Awake()
    {
        Rend = GetComponent<SpriteRenderer>();
        Health = 100;
        Provisions = _data.MaxProvisions;
        _hasMoved = false;
    }

    private void Start()
    {
        Gm = FindAnyObjectByType<GameManager>();
        Mm = FindAnyObjectByType<MapManager>();
        Um = FindAnyObjectByType<UnitManager>();
    }

    // Highlight the accessible tiles to the unit
    public void HighlightTiles()
    {
        IsSelected = true;

        // Empty to remove previous cases
        _validTiles.Clear();

        // WorlToCell takes a float postion and converts it to grid position
        Vector3Int startPos = Mm.Map.WorldToCell(transform.position);

        // You can find SeekTile() just below
        SeekTile(startPos, -1);

        foreach (var pos in _validTiles.Keys)
        {
            if (_validTiles[pos] <= Provisions)
            {
                Mm.Map.SetTileFlags(pos, TileFlags.None);
                Mm.HighlightTile(pos);
            }
            else
            {
                _validTiles.Remove(pos);
            }
        }
    }

    // Unhighlight the accessible tiles to the unit
    public void ResetTiles()
    {
        IsSelected = false;
        foreach (var pos in _validTiles.Keys)
        {
            Mm.UnHighlightTile(pos);
        }
        _validTiles.Clear();
    }

    // Check if the given grid position falls into the move range of the unit
    private bool InBounds(Vector3Int pos)
    {
        // Manhattan distance : |x1 - x2| + |y1 - y2|
        if (Mathf.Abs(Mm.Map.WorldToCell(transform.position).x - pos.x) + Mathf.Abs(Mm.Map.WorldToCell(transform.position).y - pos.y) <= _data.MoveRange)
        {
            return true;
        }
        return false;
    }


    // A recursive function to fill the ValidTiles dictionary
    private void SeekTile(Vector3Int currentPosition, int currentProvisions)
    {
        // Access the current tile
        Tile currTile = Mm.Map.GetTile<Tile>(currentPosition);
        if (currTile == null) { return; }

        if (currentProvisions < 0)
        {
            // Exception for the start tile
            currentProvisions = 0;
        }
        else
        {
            // Add the current tile fuel cost to the current fuel
            currentProvisions += Mm.GetTileData(currTile).ProvisionsCost;
        }

        if (currentProvisions > Provisions) { return; }

        // If the current tile is not an obstacle and falls into the move range of the unit
        if (!Um.IsObstacle(currentPosition, this) && InBounds(currentPosition))
        {
            if (!_validTiles.ContainsKey(currentPosition))
            {
                _validTiles.Add(currentPosition, currentProvisions);
            }
            else
            {
                if (currentProvisions < _validTiles[currentPosition])
                {
                    _validTiles[currentPosition] = currentProvisions;

                }
                else { return; }
            }
        }
        else return;


        // Explore the nighbouring tiles
        // Restrictions will be added so that we cant go out of the map
        Vector3Int up = currentPosition + Vector3Int.up;
        Vector3Int down = currentPosition + Vector3Int.down;
        Vector3Int left = currentPosition + Vector3Int.left;
        Vector3Int right = currentPosition + Vector3Int.right;

        SeekTile(up, currentProvisions);
        SeekTile(down, currentProvisions);
        SeekTile(left, currentProvisions);
        SeekTile(right, currentProvisions);
    }

    public static float L1Distance(Vector3 A, Vector3 B)
    {
        return Mathf.Abs(A.x - B.x) + Mathf.Abs(A.y - B.y) + Mathf.Abs(A.z - B.z);
    }

    public Vector3Int GetGridPosition()
    {
        return Mm.Map.WorldToCell(transform.position);
    }
}