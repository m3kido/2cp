using System;
using System.Collections.Generic;
using UnityEngine;

//this handles the game logic
public class GameManager : MonoBehaviour
{

    public static event Action OnSave;
    public static event Action OnLoad;

    public int PlayerTurn = 0;
    public int Day = 1;
    public List<Player> Players;
    public static event Action OnStateChange;
    private EGameStates _gameState;
    public EGameStates GameState
    {
        get { return _gameState; }
        set { _gameState = value; OnStateChange?.Invoke(); LastState = _gameState; }
    }
    public EGameStates LastState;

    private void Start()
    {
        GameState = EGameStates.Idle;
        // Initialize players
        Players = new List<Player>
        {
            new("Andrew",EPlayerColors.Amber, ETeams.A, null),
            new("Freya",EPlayerColors.Azure, ETeams.B, null)
        };

    }

    private void Update()
    {
        // Handle input for turn end
        if (Input.GetKeyDown(KeyCode.C)) EndTurn();
        if (Input.GetKeyDown(KeyCode.S)) OnSave.Invoke();
        if (Input.GetKeyDown(KeyCode.D)) OnLoad.Invoke();
    }

    // Declare turn end and day end events
    public static event Action OnTurnEnd;
    public static event Action OnDayEnd;

    // Method to end turn
    private void EndTurn()
    {
        PlayerTurn = (PlayerTurn + 1) % Players.Count;
        OnTurnEnd?.Invoke();
        if (PlayerTurn != 0) return;
        Day++;
        OnDayEnd?.Invoke();
    }

}