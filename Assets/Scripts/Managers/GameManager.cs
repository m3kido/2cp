using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class to handle the game logic
public class GameManager : MonoBehaviour
{
    #region Variables
    // Auto-properties (the compiler automatically creates private fields for them)
    public TextMeshProUGUI playerTurnText;
    public GameObject Turn;
    public float displayDuration = 0.5f;
    private float timer;

    private int _playerTurn = 0;
    public int Day { get; set; } = 1;
    public List<Player> Players { get; set; } = new();
    public EPlayerStates LastStateOfPlayer { get; set; }

    private EPlayerStates _currentStateOfPlayer;
    public EPlayerStates CurrentStateOfPlayer // Property for the _currentStateOfPlayer field
    {
        get { return _currentStateOfPlayer; }
        set { _currentStateOfPlayer = value; OnStateChange?.Invoke(); LastStateOfPlayer = _currentStateOfPlayer; }
    }

    // Event to let know that the state of the player has changed
    public static event Action OnStateChange;


    private void Awake()
    {
        CurrentStateOfPlayer = EPlayerStates.Idle;
        LastStateOfPlayer = EPlayerStates.Idle;
        // Initialize players
    }

    private void Start()
    {
        // Initialize players
        Players = new List<Player>
        {
            new("9999", 0, "Mohamed", ETeamColors.Amber, ETeams.A, ECaptains.Melina, 0, false),
            new("9998", 1, "Oussama", ETeamColors.Azure, ETeams.B, ECaptains.Andrew, 0, false),
        };
    }

    private void Update()
    {
        if (Turn.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer >= displayDuration)
            {
                Turn.SetActive(false);
                timer = 0f;
            }
        }

        // Handle input for turn end
        if (Input.GetKeyDown(KeyCode.C) && CurrentStateOfPlayer == EPlayerStates.Idle && !Turn.activeSelf)
        {
            EndTurn();
            Turn.SetActive(!Turn.activeSelf);
            timer = 0f;
        }
    }
    #endregion

    //public static GameManager Instance { get { return FindObjectOfType<GameManager>(); } private set { } }

    public int PlayerTurn
    {
        get
        {

            return _playerTurn;
        }

        /*set
        {
            int i = value;

            while (Players[i].Lost) // advance in Players list till finding a valid player
            {
                i = (i + 1) % Players.Count;
                OnTurnEnd?.Invoke();
                if (i == 0)
                {
                    Day++;
                    OnDayEnd?.Invoke();
                };
                OnTurnStart?.Invoke();
            }

            _playerTurn = i;
        }*/

        set
        {
            int i = value;
            while (Players[i].Lost)
            {
                i = ++i % Players.Count;
            }
            _playerTurn = i;
        }
    }

    // Declare turn end and day end events
    public static event Action OnTurnEnd;
    public static event Action OnTurnStart;
    public static event Action OnDayEnd;

    // Method to end a turn
    public void EndTurn()
    {
        PlayerTurn = (PlayerTurn + 1) % Players.Count;
        playerTurnText.text = "Player " + (PlayerTurn + 1) + "'s turn";
        OnTurnEnd?.Invoke();
        if (PlayerTurn == 0)
        {
            Day++;
            OnDayEnd?.Invoke();
        };


        OnTurnStart?.Invoke();
    }

    /*public void EndTurn()
    {
        PlayerTurn = (PlayerTurn + 1) % Players.Count;
    }*/

    public void CheckGameStatus()
    {
        //Players[]
    }

    /*public void RemovePlayer(Player player)
    {
        player.RemoveCaptain();
        Players.Remove(player);
    }*/

}