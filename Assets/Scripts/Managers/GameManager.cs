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
    [SerializeField] List<Player> _players;
    public List<Player> Players { get => _players; set => _players = value; }
     
    public static event Action OnStateChange;
    private EGameStates _gameState;
    public EGameStates GameState
    {
        get { return _gameState; }
        set { _gameState = value; OnStateChange?.Invoke(); LastState = _gameState; }
    }
    public EGameStates LastState;

    [SerializeField] private Captain captainA;
    [SerializeField] private Captain captainB;

    private void Start()
    {
        
        GameState = EGameStates.Idle;
        // Initialize players

        /*Players = new List<Player>
        {
            new("Andrew",EPlayerColors.Amber, ETeams.A, captainA),
            new("Freya",EPlayerColors.Azure, ETeams.B, captainB)
        };*/

    }

    private void Update()
    {
      
            // Handle input for turn end
            if (Input.GetKeyDown(KeyCode.C)) EndTurn();

            // Check if there are subscribers before invoking the events
            if (Input.GetKeyDown(KeyCode.S) && OnSave != null)
                OnSave.Invoke();

            if (Input.GetKeyDown(KeyCode.D) && OnLoad != null)
                OnLoad.Invoke();
   
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