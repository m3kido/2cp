public class Captain
{
    public CaptainDataSO Data;
    public Player Player;
    public bool IsCelesteActive = false;
    public int PassiveAttack;
    public int PassiveDefense;
    public float AttackMultiplier = 1.0f;
    public float DefenseMultiplier = 1.0f;
    public int AttackRangeAdditioner = 0;
    public float CaptureMultiplier = 1.0f;
    public float PriceMultiplier = 1.0f;
    public int SuperMeter { get; set; } = 100;




    public virtual void EnableCeleste()
    {
        if (!IsCelesteReady())
            return;
        IsCelesteActive = true;
        SuperMeter -= 100;
    }

    public virtual void DisableCeleste()
    {
        if (!IsCelesteActive) { return; }
        IsCelesteActive = false;
    }

    public bool IsCelesteReady()
    {
        return (SuperMeter == 100);
    }

    public Captain(Player player)
    {
        Player = player;
        CaptainManager.LivingCaptains.Add(this);
    }

    public virtual void UnsubscribeWhenDestroyed()
    {

    }

}
