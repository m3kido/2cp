// Class to represent a player
public class Player
{
    // Auto-properties (the compiler automatically creates private fields for them)
    public string Name { get; set; }
    public bool Lost { get; set; }
    public ETeamColors Color { get; set; }
    public ETeams TeamSide { get; set; }
    public Captain PlayerCaptain { get; set; }
    public int Gold { get; set; }
    public int SuperMeter { get; set; }

    // Player constructor
    public Player(string name, ETeamColors color, ETeams teamSide, Captain captain)
    {
        Name = name;
        Color = color;
        TeamSide = teamSide;
        PlayerCaptain = captain;
    }
}
