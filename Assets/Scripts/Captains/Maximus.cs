public class Maximus : Captain
{
    public Maximus(Player player) : base(player)
    {
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Maximus];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
    }
    public override void EnableCeleste()
    {
        base.EnableCeleste();

        DefenseMultiplier -= 0.2f;
        AttackMultiplier += 0.25f;

    }

    public override void DisableCeleste()
    {
        base.DisableCeleste();
        DefenseMultiplier -= 0.2f;
        AttackMultiplier += 0.25f;

    }
    public override bool IsCelesteReady()
    {
        return base.IsCelesteReady();
    }
}
