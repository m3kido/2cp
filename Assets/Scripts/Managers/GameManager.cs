using System;
using System.Collections.Generic;
using UnityEngine;

// Class to handle the game logic
public class GameManager : MonoBehaviour
{
    #region Variables
    // Auto-properties (the compiler automatically creates private fields for them)
    public int PlayerTurn { get; set; }
    public int Day { get; set; } = 1;
    public List<Player> Players { get; set; }
    public EPlayerStates LastStateOfPlayer { get; set; }

    private EPlayerStates _currentStateOfPlayer;
    public EPlayerStates CurrentStateOfPlayer // Property for the _currentStateOfPlayer field
    { 
        get { return _currentStateOfPlayer; } 
        set { _currentStateOfPlayer = value; OnStateChange?.Invoke(); LastStateOfPlayer = _currentStateOfPlayer; }
    }

    // Event to let know that the state of the player has changed
    public static event Action OnStateChange;

    // Turn end and day end events
    public static event Action OnTurnEnd;
    public static event Action OnDayEnd;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        CurrentStateOfPlayer = EPlayerStates.Idle;
        LastStateOfPlayer = EPlayerStates.Idle;
        // Initialize players
        Players = new List<Player>
        {
            new("Mohamed", ETeamColors.Amber, ETeams.A, ECaptains.Andrew,null),
            new("Oussama", ETeamColors.Azure, ETeams.B, ECaptains.Maximus, null)
        };
    }
   
    private void Update()
    {
        // Handle input for turn end
        if (Input.GetKeyDown(KeyCode.C)) EndTurn();
    }
    #endregion

    #region Methods
    // Method to end a turn
    public void EndTurn()
    {
        PlayerTurn = (PlayerTurn + 1) % Players.Count;
        OnTurnEnd?.Invoke();
        if (PlayerTurn != 0) return;
        Day++;
        OnDayEnd?.Invoke();
    }
    #endregion
}