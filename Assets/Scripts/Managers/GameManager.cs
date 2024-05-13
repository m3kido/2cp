using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class to handle the game logic
public class GameManager : MonoBehaviour
{
    #region Variables
    // public static GameManager Instance { get { return FindObjectOfType<GameManager>(); } private set { } }

    public TextMeshProUGUI PlayerTurnText;
    public GameObject Turn;
    public GameObject winUi;
    public TextMeshProUGUI WinnerText;
    public float DisplayDuration = 0.5f;
    public float EndGameDuration = 2f;
    private float _timer;
    private float _endtimer;

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

    public int PlayerTurn
    {
        get
        {
            return _playerTurn;
        }

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
    #endregion

    #region UnityMethods 
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
            new("9999", 0, "Mohamed", ETeamColors.Amber, ETeams.A, ECaptains.Andrew, 0, false),
            new("9998", 1, "Oussama", ETeamColors.Azure, ETeams.B, ECaptains.Melina, 0, false),
        };
    }

    private void Update()
    {
        if (Turn.activeSelf)
        {
            _timer += Time.deltaTime;
            if (_timer >= DisplayDuration)
            {
                Turn.SetActive(false);
                _timer = 0f;
            }
        }

        // Handle input for turn end
        if (Input.GetKeyDown(KeyCode.C) && CurrentStateOfPlayer == EPlayerStates.Idle && !Turn.activeSelf)
        {
            EndTurn();
            Turn.SetActive(true);
            _timer = 0f;
        }

        (bool isGameOver, int winnerIdx) = IsGameOver();

        if (isGameOver)
        {
            EndGame(winnerIdx);
          /*_endtimer += Time.deltaTime;
             if (_endtimer >= EndGameDuration)
             {
                 SceneManager.LoadScene("Main Menu");
                 _endtimer = 0f;
             }*/
        }
    }
    #endregion

    #region Methods
    // Method to end a turn
    public void EndTurn()
    {
        PlayerTurn = (PlayerTurn + 1) % Players.Count;
        PlayerTurnText.text = "Player " + (PlayerTurn + 1) + "'s turn";
        OnTurnEnd?.Invoke();
        if (PlayerTurn == 0)
        {
            Day++;
            OnDayEnd?.Invoke();
        };


        OnTurnStart?.Invoke();
    }

    public (bool, int) IsGameOver()
    {
        int activePlayersCount = 0;
        int idx = 0;
        foreach (var player in Players)
        {
            if (!player.Lost)
            {
                activePlayersCount++;
                idx = player.PlayerNumber;
            }
        }
        return (activePlayersCount == 1, idx);
    }

    public void EndGame(int playerIndex)
    {
        CurrentStateOfPlayer= EPlayerStates.WinScreen;
        WinnerText.text = Players[playerIndex].PlayerCaptain.CaptainName + " Wins";
        winUi.SetActive(true);
    }

    /*public void RemovePlayer(Player player)
    {
        player.RemoveCaptain();
        Players.Remove(player);
    }*/
    #endregion
}