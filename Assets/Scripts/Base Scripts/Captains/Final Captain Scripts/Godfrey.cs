public class Godfrey : Captain
{
    public Godfrey(Player player) : base(player)
    {
        CaptainName = ECaptains.Godfrey;
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Godfrey];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
        DefenseMultiplier += 0.05f;
        GameManager.OnDayEnd += DisableCeleste;
        maxSuperMeter = 30000;
        SuperMeter = maxSuperMeter / 2;
    }

    public override void EnableCeleste()
    {
        if (!IsCelesteReady())
            return;
        base.EnableCeleste();
        SuperMeter -= maxSuperMeter;
        foreach (var unit in CaptainManager.Um.Units)
        {
            if (CaptainManager.Gm.Players[unit.Owner] != Player)
            {
                unit.MoveRange--;
                // Increase provisions
            }
        }
    }

    public override void DisableCeleste()
    {
        if (!IsCelesteActive) { return; }
        base.DisableCeleste();
        foreach (var unit in CaptainManager.Um.Units)
        {
            if (CaptainManager.Gm.Players[unit.Owner] != Player)
            {
                unit.MoveRange = unit.Data.MoveRange ;
            }
        }
    }

    public override void UnsubscribeWhenDestroyed()
    {
        GameManager.OnTurnStart -= DisableCeleste;
    }
}
