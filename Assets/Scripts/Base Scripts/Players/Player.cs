// Class to represent a player
public class Player
{
    public string ID { get; private set; }
    public int PlayerNumber { get; set; }
    public string Name { get; set; }
    private bool _lost = false;
    public ETeamColors Color { get; set; }
    public ETeams TeamSide { get; set; }
    public Captain PlayerCaptain { get; set; }

    public int Gold { get; set; }

    public bool Lost
    {
        get => _lost;
        set
        {
            // if (value == true) GameManager.Instance.RemovePlayer(this);
            if (value == true)
                foreach (var unit in UnitManager.Instance.Units)
                {
                    if (unit.Owner == PlayerNumber) UnitManager.Instance.DestroyUnit(unit);
                }
            _lost = value;
        }
    }

    // Player constructor
    public Player(string id, int number, string name, ETeamColors color, ETeams teamSide, ECaptains captain, int gold, bool lost)
    {
        ID = id;
        PlayerNumber = number;
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

        Gold = gold;
        Lost = lost;
    }

    public void RemoveCaptain()
    {
        CaptainManager.DeleteCaptain(PlayerCaptain);
    }

    public PlayerSaveData GetDataToSave()
    {
        return new PlayerSaveData(ID, PlayerNumber, Name, Color, TeamSide,
            new CaptainSaveData(PlayerCaptain.CaptainName, PlayerCaptain.IsCelesteActive, PlayerCaptain.SuperMeter),
            Gold, Lost);
    }
}