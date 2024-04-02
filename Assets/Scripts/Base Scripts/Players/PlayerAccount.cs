public class PlayerAccount
{
    public string PlayerID { get; private set; }
    public string RealName { get; private set; }
    public string Nickname { get; set; }
    public int Gold { get; set; }

    public PlayerAccount(string realName, string nickname)
    {
        PlayerID = System.Guid.NewGuid().ToString();
        RealName = realName;
        Nickname = nickname;
    }
}
