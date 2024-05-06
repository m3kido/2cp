public class Maximus : Captain
{
    public Maximus(Player player) : base(player)
    {
        CaptainName = ECaptains.Maximus;
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Maximus];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
        CaptureMultiplier += 0.2f;
        AttackRangeAdditioner = 0;
        GameManager.OnTurnEnd += DisableCeleste;
    }
    public override void EnableCeleste()
    {
        base.EnableCeleste();

        DefenseMultiplier -= 0.2f;
        AttackMultiplier += 0.25f;
        AttackRangeAdditioner++; 
        UnityEngine.Debug.Log("Maximus");

    }

    public override void DisableCeleste()
    {
        if (!IsCelesteActive) { return; }
        base.DisableCeleste();
        DefenseMultiplier += 0.2f;
        AttackMultiplier -= 0.25f;
        AttackRangeAdditioner--;

    }
    public override void UnsubscribeWhenDestroyed()
    {
        GameManager.OnTurnEnd -= DisableCeleste;
    }
}
