public class Captain
{
    public CaptainDataSO Data;
    public Player Player;
    public bool IsCelesteActive = false;
    public int PassiveAttack;
    public int PassiveDefense;
    public float AttackMultiplier = 1.0f;
    public float DefenseMultiplier = 1.0f;
    private int _attackRangeAdditioner = 0;
    public float CaptureMultiplier = 1.0f;
    public float PriceMultiplier = 1.0f;
    public int SuperMeter { get; set; } = 100;


    public int AttackRangeAdditioner
    {
        get
        {
            return _attackRangeAdditioner;
        }
        set {
            _attackRangeAdditioner = value;
            UnityEngine.Debug.Log("AttackRangeAdditioner : " + value);
        }
    }

    public virtual void EnableCeleste()
    {
        if (!IsCelesteReady())
            return;
        IsCelesteActive = true;
        SuperMeter -= 100;
    }

    public virtual void DisableCeleste()
    {
        
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