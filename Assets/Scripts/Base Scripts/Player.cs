// Class to represent a player
public class Player
{
    public string Name { get; set; }
    public bool Lost { get; set; }
    public ETeamColors Color { get; set; }
    public ETeams TeamSide { get; set; }
    public Captain PlayerCaptain { get; set; }
    public float CPCharge { get; set; } // CP = Celestial Power
    public bool IsCPActivated { get; set; }
    public int Gold { get; set; }

    // Player constructor
    public Player(string name, ETeamColors color, ETeams teamSide, Captain captain)
    {
        Name = name;
        Color = color;
        TeamSide = teamSide;
        PlayerCaptain = captain;
    }
}
