public class Andrew : Captain
{
    public Andrew(Player player) : base(player)
    {
        CaptainName = ECaptains.Andrew;
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Andrew];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
        maxSuperMeter = 40000;
        SuperMeter = maxSuperMeter / 2;
        GameManager.OnTurnEnd += DisableCeleste;
    }

    public override void EnableCeleste()
    {
        if (!IsCelesteReady())
            return;
        base.EnableCeleste();
        SuperMeter -= maxSuperMeter; 
        foreach (var unit in CaptainManager.Um.Units)
        {
            if (CaptainManager.Gm.Players[unit.Owner] == Player)
            {
                unit.Health += (int)(0.2 * Unit.MaxHealth);
                CaptainManager.Instance.HealSpr(unit);
            }
        }
        
        DefenseMultiplier += 0.2f;
    }

    public override void DisableCeleste()
    {
        if (!IsCelesteActive) { return; }
        base.DisableCeleste();
        DefenseMultiplier -= 0.2f;
    }

    public override void UnsubscribeWhenDestroyed()
    {
        GameManager.OnTurnEnd -= DisableCeleste;
    }
}
