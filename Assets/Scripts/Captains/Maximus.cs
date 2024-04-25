public class Maximus : Captain
{
    public Maximus(Player player) : base(player)
    {
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Maximus];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
        CaptureMultiplier += 0.2f;
        GameManager.OnTurnEnd += DisableCeleste;
    }
    public override void EnableCeleste()
    {
        base.EnableCeleste();

        DefenseMultiplier -= 0.2f;
        AttackMultiplier += 0.25f;
        AttackRangeAdditioner += 1;
        UnityEngine.Debug.Log("Maximus");

    }

    public override void DisableCeleste()
    {
        base.DisableCeleste();
        DefenseMultiplier += 0.2f;
        AttackMultiplier -= 0.25f;
        AttackRangeAdditioner = -1;

    }
    public override void UnsubscribeWhenDestroyed()
    {
        GameManager.OnTurnEnd -= DisableCeleste;
    }
}
