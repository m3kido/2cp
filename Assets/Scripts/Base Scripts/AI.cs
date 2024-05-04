/*using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class to represent an AI player
public class AiPlayer : Player
{

    GameManager _gm;
    UnitManager _um;
    BuildingManager _bm;
    MapManager _mm;
    AttackManager _am; 
    List<Unit> AiUnits;
    int AiLavel; 

    // Constructor for AI player
    public AiPlayer(string name, ETeamColors color, ETeams teamSide, ECaptains captain)
        : base(name, color, teamSide, captain)
    {
        // Initialize any additional properties or settings specific to AI players if needed
    }

    //Unity Methods 
    private void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();   
        _um = FindAnyObjectByType<UnitManager>();
        _bm = FindAnyObjectByType<BuildingManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _am = FindAnyObjectByType<AttackManager>();
    }

    private void Start()
    {
        foreach(var unit in _um.Units)
        {
            if(_gm.Players[unit.Owner] == this)
            {
                AiUnits.Add(unit);
            }
        }
    }

    void Update()
    {

    }

    public void AiPlay(List<Unit> allUnits, List<Unit> enemyUnits)
    {
        //Level 0 STUID ASU
        
        // Decide which units to move and where to move them
        
        // Example: AI decides to end its turn after performing actions
        EndTurn();
    }

    //Worth attack 
    private bool worthAttack(AttackingUnit attacker , Unit enemy , float threshold  )
    {
        float evaluation = float.MinValue;
        float wdc = 0.45f, wdr = 0.25f, wp = 0.3f;
        float DC = _am.CalculateDamage(enemy, attacker);
        float DR = _am.checkCounterAttack(enemy, attacker); return false ;
        float P = attacker.Data.Cost;
        evaluation = wdc * (DC / 100) - wdr * (DR / 100) - wp * ((P - 5) / (28 - 5)); 
        if(evaluation < threshold || DC-DR <= 0  ) return false ;
        return true; 
    }

    // Function to find the nearest enemy appraoch 
    
    private List<Unit> FindNearestEnemy(Unit unit, List<Unit> enemyUnits)
    {
        List<(Unit, float)> enemies = new List<(Unit, float)>();
        Unit nearestEnemy = null;
        float minDistance = float.MaxValue;
        var attackerPos = unit.GetGridPosition();
        foreach (Unit enemy in enemyUnits)
        {
            if(unit is AttackingUnit)
            {
                AttackingUnit attacker = unit as AttackingUnit;
                var potentialTargetPos = _mm.Map.WorldToCell(unit.transform.position);
                float distance = 0;
                bool IsInRange = (attacker.L2Distance2D(attackerPos, potentialTargetPos) >= attacker.Weapons[attacker.CurrentWeaponIndex].MinRange) && (attacker.L2Distance2D(attackerPos, potentialTargetPos) < (attacker.Weapons[attacker.CurrentWeaponIndex].MaxRange + this.PlayerCaptain.AttackRangeAdditioner));
                if (IsInRange && worthAttack(attacker, enemy, 0.4f))
                {
                    distance = unit.L2Distance2D(attackerPos, potentialTargetPos);
                    enemies.Add((enemy, distance));

                }
            }
            
        }

        // Sort evaluated enemies by evaluation value in descending order
        enemies.Sort((x, y) => y.Item2.CompareTo(x.Item2));

        // Extract best enemies from the sorted list
        List<Unit> nearestEnemies = enemies.Select(item => item.Item1).ToList();
        return nearestEnemies;
    }

    private List<Unit> FindBestEnemies(Unit unit, List<Unit> enemyUnits)
    {
        List<(Unit, float)> evaluatedEnemies = new List<(Unit, float)>();

        var attackerPos = unit.GetGridPosition();
        foreach (Unit enemy in enemyUnits)
        {
            if (unit is AttackingUnit)
            {
                AttackingUnit attacker = unit as AttackingUnit;
                if (worthAttack(attacker, enemy, 0.4f))
                {
                    var potentialTargetPos = _mm.Map.WorldToCell(unit.transform.position);
                    float DC = _am.CalculateDamage(enemy, attacker);
                    float DR = _am.checkCounterAttack(enemy, attacker);
                    float distance = attacker.L2Distance2D(attackerPos, potentialTargetPos);
                    float evaluation = 0.6f * DC - 0.3f * DR - 0.1f * distance;
                    evaluatedEnemies.Add((enemy, evaluation));
                }
            }
        }

        // Sort evaluated enemies by evaluation value in descending order
        evaluatedEnemies.Sort((x, y) => y.Item2.CompareTo(x.Item2));

        // Extract best enemies from the sorted list
        List<Unit> bestEnemies = evaluatedEnemies.Select(item => item.Item1).ToList();

        return bestEnemies;
    }


    private bool actionDecision(Unit unit)
    {
        List<Unit> enemyUnits;
        if (unit is AttackingUnit)
        {

            AttackingUnit attacker = unit as AttackingUnit;
            if (attacker == null) return false;
            var attackerPos = attacker.GetGridPosition();
            attacker.SeekTile(attackerPos, -1, 0);
            enemyUnits = attacker.ScanTargets(unit.Data.MoveRange);
            switch (AiLavel)
            {
                case 1:

                    List<Unit> nearestEnemies = FindNearestEnemy(attacker, enemyUnits);
                    foreach (var enemy in nearestEnemies)
                    {
                        var enemyPosition = enemy.GetGridPosition();
                        var adjacentPositions = new[]
                            {
                                enemyPosition + Vector3Int.up,
                                enemyPosition + Vector3Int.down ,
                                enemyPosition + Vector3Int.left,
                                enemyPosition + Vector3Int.right ,
                            };

                        foreach (var position in adjacentPositions)
                        {
                            if (attacker.ValidTiles.ContainsKey(position))
                            {

                            }
                        }
                    }

                    break;

                case 2:
                    
                    List<Unit> bestEnemies = FindBestEnemies(attacker, enemyUnits);
                    foreach (var enemy in bestEnemies)
                    {
                        var enemyPosition = enemy.GetGridPosition();
                        var adjacentPositions = new[]
                            {
                                enemyPosition + Vector3Int.up,
                                enemyPosition + Vector3Int.down ,
                                enemyPosition + Vector3Int.left,
                                enemyPosition + Vector3Int.right ,
                            };
                        foreach (var position in adjacentPositions)
                        {
                            if (attacker.ValidTiles.ContainsKey(position))
                            {
                                //WE NEED TO DECIDE TO WHICH TILE WE WILL MOVE
                                //GO TO THAT UNIT USING THE PATHFINDER 
                                //ATTACK 
                            }
                        }  
                    }
                    break;
            }
            
        }
        return false;
    }

    // Method to end AI player's turn
    private void EndTurn()
    {
        // Perform any cleanup or end-of-turn actions here
        Console.WriteLine("AI player " + Name + " has ended its turn.");
    }
}
*/