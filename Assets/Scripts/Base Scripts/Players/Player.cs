// Class to represent a player
using System;

public class Player 
{
    // Auto-properties (the compiler automatically creates private fields for them)
    public string Name { get; set; }
    public bool Lost { get; set; }
    public ETeamColors Color { get; set; }
    public ETeams TeamSide { get; set; }
    public Captain PlayerCaptain { get; set; }
    
    public int Gold { get; set; }
    

    // Player constructor
    public Player(string name, ETeamColors color, ETeams teamSide, ECaptains captain)
    {
        Name = name;
        Color = color;
        TeamSide = teamSide;
        switch (captain)
        {
            case ECaptains.Andrew:
                PlayerCaptain = new Andrew(this);
                break;
            case ECaptains.Godfrey:
                PlayerCaptain = new Godfrey(this);
                break;
            case ECaptains.Maximus:
                PlayerCaptain = new Maximus(this);
                break;
            case ECaptains.Melina:
                PlayerCaptain = new Melina(this);
                break;
        }
    }

    public void RemoveCaptain()
    {
        CaptainManager.DeleteCaptain(PlayerCaptain);
    }
    
}
