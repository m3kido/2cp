using System.Collections.Generic;

// Class to represent a team of players (allies)
public class Team
{
    // Auto-property (the compiler automatically creates a private field to it)
    public List<Player> TeamMates { get; set; }
}