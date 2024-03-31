using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorController : MonoBehaviour
{
    UnitManager Um;
    MapManager Mm;
    BuildingManager Bm;
    GameManager Gm;

    public Vector3Int HoverTile
    {
        get => Mm.Map.WorldToCell(transform.position);
        set => transform.position = value;
    }
    public Vector3Int SaveTile;

    void Start()
    {
        // Get the unit, map and game managers from the hierarchy
        Um = FindAnyObjectByType<UnitManager>();
        Mm = FindAnyObjectByType<MapManager>();
        Gm = FindAnyObjectByType<GameManager>();
        Bm= FindAnyObjectByType<BuildingManager>();
    }

    void Update()
    {
        // Handle input every frame
        if(Gm.GameState==EGameStates.Idle || Gm.GameState==EGameStates.Selecting) {
            HandleInput();
        }
        
    }

    void HandleInput()
    {
        // Dont handle any input if a unit is moving or attacking
        if (Um.SelectedUnit!= null && Um.SelectedUnit.IsMoving) { return; }

        // X key
        if (Input.GetKeyDown(KeyCode.X))
        {
            XClicked();
        }

        // Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpaceClicked();
        }

        // Arrow keys
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelector(Vector3Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelector(Vector3Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelector(Vector3Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelector(Vector3Int.down);
        }
    }

    // Move the cursor 
     void MoveSelector(Vector3Int offset)
    {
        // Dont let the cursor move out of the highlited tiles
        if (Um.SelectedUnit != null && !Um.SelectedUnit.ValidTiles.ContainsKey(HoverTile + offset))
        {
            return;
        }

        // If a unit is selected, record the path
        if (Um.SelectedUnit != null)
        {
            // Undraw the path if we get back the start point
            if (Um.SelectedUnit.transform.position == HoverTile + offset)
            {
                Um.UnDrawPath();
                Um.Path.Clear();
                Um.PathCost = 0;
            }
            else
            {
                int index = Um.Path.IndexOf(HoverTile + offset); // Returns -1 if not found
                if (index < 0)
                {
                    // Add tile to path
                    int cost = Mm.GetTileData(Mm.Map.GetTile<Tile>(HoverTile + offset)).FuelCost;
                    if (Um.PathCost + cost > Um.SelectedUnit.Fuel) { return; }
                    Um.UnDrawPath();
                    Um.Path.Add(HoverTile + offset);
                    Um.PathCost += cost;
                }
                else
                {
                    // Remove the loop
                    Um.UnDrawPath();
                    Um.Path.RemoveRange(index + 1, Um.Path.Count - index - 1);

                    // Recalculate the new fuel cost
                    Um.PathCost = 0;
                    foreach (Vector3Int pos in Um.Path)
                    {
                        Um.PathCost += Mm.GetTileData(Mm.Map.GetTile<Tile>(pos)).FuelCost;
                    }

                }
            }
            Um.DrawPath();
        }
        HoverTile += offset;
    }

    // Handle X Clicked
    private void XClicked()
    {
      
            if(Gm.GameState== EGameStates.Selecting) {
                    // Cancel select
                    HoverTile = Mm.Map.WorldToCell(Um.SelectedUnit.transform.position);
                    Um.DeselectUnit();
                    
            }
    }

    // Handle Space click
    private void SpaceClicked()
    {
        // Check if the game state is in attacking phase and selecting a target
        //if (Gm.GameState == EGameStates.Attacking)
        //{
        //    // Check if there is a unit on the hovered tile
        //    Unit targetUnit = Um.FindUnit(HoverTile);

        //    // If there is a unit on the hovered tile
        //    if (targetUnit != null)
        //    {
        //        // Check if the hovered unit belongs to the opponent
        //        if (targetUnit.Owner != Gm.PlayerTurn)
        //        {
        //            //JUST HIGHLITING THIS UNIT SO IKK EVRYTHING IS WORKING 
        //            if (targetUnit.TryGetComponent<Renderer>(out var renderer))
        //            {
        //                MaterialPropertyBlock propBlock = new();
        //                renderer.GetPropertyBlock(propBlock);
        //                propBlock.SetColor("_Color", Color.blue); // Set the color to blue or any desired color
        //                renderer.SetPropertyBlock(propBlock);
        //            }
        //            Gm.GameState = EGameStates.Idle;
        //        }
        //    }
        //}
        // Check if the game state is in selecting unit phase
        
            // Check if there is a unit on the hovered tile
            Unit refUnit = Um.FindUnit(HoverTile);

            // If there is a unit on the hovered tile
            if (refUnit != null)
            {
                // Can't select another unit when one is already selected
                if (Um.SelectedUnit != null && Um.SelectedUnit == refUnit)
                {
                    StartCoroutine(Um.MoveUnit());
                }
                // Check if the hovered unit belongs to the current player and hasn't moved yet
                else if (refUnit.Owner == Gm.PlayerTurn && !refUnit.HasMoved)
                {
                    SaveTile = HoverTile;
                    Um.SelectUnit(refUnit);
                }
            }
            // If there is no unit on the hovered tile
            else
            {
                // Check if a unit is already selected
                if (Um.SelectedUnit != null)
                {
                    // Move the selected unit towards the hovered tile
                    StartCoroutine(Um.MoveUnit());
                }
                else
                {
                    // Check if the hovered tile contains a building and spawn a unit if it does
                    if (Bm.Buildings.ContainsKey(HoverTile))
                    {
                        Bm.SpawnUnit(EUnitType.Infantry, Bm.Buildings[HoverTile], Gm.PlayerTurn);
                    }
                }
            }
        }
    



}
