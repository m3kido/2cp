public class Andrew : Captain
{
    public Andrew(Player player) : base(player)
    {

        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Andrew];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
    }

    public override void EnableCeleste()
    {
        base.EnableCeleste();
        foreach (var unit in CaptainManager.Um.Units)
        {
            if (CaptainManager.Gm.Players[unit.Owner] == Player)
            {

                unit.Health += (int)(0.2 * Unit.MaxHealth);
                
            }
        }
        DefenseMultiplier += 0.2f;
    }

    public override void DisableCeleste()
    {
        base.DisableCeleste();
        DefenseMultiplier -= 0.2f;

    }
    public override bool IsCelesteReady()
    {
        return base.IsCelesteReady();
    }

}
