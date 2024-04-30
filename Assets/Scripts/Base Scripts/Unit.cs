using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Class to represent a unit, associated to every unit prefab on the scene
public class Unit : MonoBehaviour
{
    #region Variables
    // Managers will be needed
    protected MapManager _mm;
    protected UnitManager _um;
    protected GameManager _gm;
    public SpriteRenderer _rend;

    [SerializeField] private UnitDataSO _data;
    public UnitDataSO Data => _data; // Readonly property for the _data field

    // Auto-properties (the compiler automatically creates private fields for them)
    private int _health; // { get; set; }
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
                _rend.color = Color.gray;
            }
            else
            {
                _rend.color = Color.white;
            }
        }
    }


    public int Health
    {
        get
        {
            return _health;
        }

        set
        {
            if (value <= 0)
            {
                _health = 0;
                Die();
            }
            else if (value > MaxHealth)
            {

                _health = MaxHealth;
            }
            else
            {
                _health = value;
            }
        }
    }
    public static int MaxHealth = 100;
    public int MoveRange;
    // Dictionary to hold the grid position of the valid tiles along with the fuel consumed to reach them

    private Dictionary<Vector3Int, int> _validTiles = new();
    public Dictionary<Vector3Int, int> ValidTiles
    {
        get => _validTiles;
        set => _validTiles = value;
    }

    private void Awake()
    {
        // Get map and unit manager from the hierarchy
        _mm = FindAnyObjectByType<MapManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _rend = GetComponent<SpriteRenderer>();
        Health = MaxHealth;
        Provisions = _data.MaxProvisions;
        _hasMoved = false;
        MoveRange = Data.MoveRange;
    }

    private void Start()
    {
        // Get map and unit manager from the hierarchy
        _mm = FindAnyObjectByType<MapManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _gm = FindAnyObjectByType<GameManager>();
        AssignColor();
    }

    private void AssignColor()
    {
        if (_gm == null || _gm.Players == null)
        {
            // Handle null references gracefully
            Debug.LogError("ERROR: GameManager or Players list is null.");
            return;
        }

        if (_owner < 0 || _owner >= _gm.Players.Count)
        {
            // Handle invalid owner index gracefully
            Debug.LogError("ERROR: Invalid owner index.");
            return;
        }

        ETeamColors OwnerColor = _gm.Players[_owner].Color;

        Color OutlineColor;
        switch (OwnerColor)
        {
            case ETeamColors.Amber: OutlineColor = Color.red; break;
            case ETeamColors.Azure: OutlineColor = Color.blue; break;
            case ETeamColors.Gilded: OutlineColor = Color.yellow; break;
            case ETeamColors.Verdant: OutlineColor = Color.green; break;
            default: OutlineColor = Color.clear; break;
        }
        GetComponent<SpriteRenderer>().material.color = OutlineColor;
    }


    // Highlight the accessible tiles to the unit
    public void HighlightTiles()
    {
        IsSelected = true;

        // Empty to remove previous cases
        ValidTiles.Clear();


        // WorlToCell takes a float postion and converts it to grid position
        Vector3Int startPos = _mm.Map.WorldToCell(transform.position);

        // You can find SeekTile() just below
        SeekTile(startPos, -1);

        foreach (var pos in ValidTiles.Keys)
        {
            if (ValidTiles[pos] <= Provisions)
            {
                _mm.Map.SetTileFlags(pos, TileFlags.None);
                _mm.HighlightTile(pos);
            }
            else
            {
                ValidTiles.Remove(pos);
            }
        }
        
      
       
        
    }

    // Unhighlight the accessible tiles to the unit
    public void ResetTiles()
    {
        IsSelected = false;
        foreach (var pos in ValidTiles.Keys)
        {
            _mm.UnHighlightTile(pos);
        }
        ValidTiles.Clear();
    }

    // Check if the given grid position falls into the move range of the unit
    private bool InBounds(Vector3Int pos)
    {
        // Manhattan distance : |x1 - x2| + |y1 - y2|
        if (Mathf.Abs(_mm.Map.WorldToCell(transform.position).x - pos.x) + Mathf.Abs(_mm.Map.WorldToCell(transform.position).y - pos.y) <= _data.MoveRange)
        {
            return true;
        }
        return false;
    }

    // A recursive function to fill the ValidTiles dictionary
    protected void SeekTile(Vector3Int currentPosition, int currentProvisions)
    {
        // Access the current tile
        Tile currTile = _mm.Map.GetTile<Tile>(currentPosition);
        if (currTile == null) { return; }

        if (currentProvisions < 0)
        {
            // Exception for the start tile
            currentProvisions = 0;
        }
        else
        {
            // Add the current tile fuel cost to the current fuel
            currentProvisions += _mm.GetTileData(currTile).ProvisionsCost;
        }

        if (currentProvisions > Provisions) { return; }

        // If the current tile is not an obstacle and falls into the move range of the unit
        if (!_um.IsObstacle(currentPosition, this) && InBounds(currentPosition))
        {
            if (!ValidTiles.ContainsKey(currentPosition))
            {
                ValidTiles.Add(currentPosition, currentProvisions);
            }
            else
            {
                if (currentProvisions < ValidTiles[currentPosition])
                {
                    ValidTiles[currentPosition] = currentProvisions;

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
    public void Die()
    {
        print("I'm Going To Die!");
        _um.Units.Remove(this);
        Destroy(gameObject);

    }

    public static float L1Distance2D(Vector3 A, Vector3 B)
    {
        return Mathf.Abs(A.x - B.x) + Mathf.Abs(A.y - B.y);//+ Mathf.Abs(A.z - B.z)
    }

    public Vector3Int GetGridPosition()
    {
        return _mm.Map.WorldToCell(transform.position);
    }

    public Captain GetUnitCaptain
    {
        get
        {
            var player = _gm.Players[Owner];
            return player.PlayerCaptain;
        }

        private set { }

    }

}

#endregion