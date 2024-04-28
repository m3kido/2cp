// Class to represent a player
public class Player
{
    // Auto-properties (the compiler automatically creates private fields for them)
    public string Name { get; set; }
    public bool Lost { get; set; }
    public ETeamColors Color { get; set; }
    public ETeams TeamSide { get; set; }
    public ECaptains PlayerCaptain { get; set; }
    public Captain Captain { get; set; }
    public int Gold { get; set; }
    public int SuperMeter { get; set; }

    // Player constructor
    public Player(string name, ETeamColors color, ETeams teamSide, ECaptains captain, Captain playerCaptain)
    {
        Name = name;
        Color = color;
        TeamSide = teamSide;
        PlayerCaptain = captain;
        this.Captain = playerCaptain;
    }
}
