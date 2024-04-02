public class PlayerInGame
{
    public string PlayerID { get; private set; }
    public int PlayerNumber { get; set; } // {1, 2, 3, 4}
    public bool Lost { get; set; }
    public ETeamColors Color { get; set; }
    public ETeams TeamSide { get; set; }
    public Captain PlayerCaptain { get; set; }
    public float CPCharge { get; set; } // CP = Celestial Power
    public bool IsCPActivated { get; set; }
    public int Gold { get; set; }

    // Player constructor
    public PlayerInGame(string playerID, int playerNumber,
        ETeamColors color, ETeams teamSide, Captain playerCaptain)
    {
        PlayerID = playerID;
        PlayerNumber = playerNumber;
        Color = color;
        TeamSide = teamSide;
        PlayerCaptain = playerCaptain;
    }

    public PlayerSaveData GetDataToSave()
    {
        return new PlayerSaveData(PlayerID, PlayerNumber, Color, TeamSide, PlayerCaptain, CPCharge, IsCPActivated, Gold);
    }
}
