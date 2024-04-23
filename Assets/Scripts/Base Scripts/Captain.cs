public class Captain
{
    public CaptainDataSO Data;
    public Player Player;
    public bool IsCelesteActive = false;
    public int PassiveAttack;
    public int PassiveDefense;
    public float AttackMultiplier = 1.0f;
    public float DefenseMultiplier = 1.0f;


    
    public virtual void EnableCeleste()
    {
        IsCelesteActive = true;
        
    }

    public virtual void DisableCeleste()
    {
        IsCelesteActive = false;
    }

    public virtual bool IsCelesteReady()
    {
        return false;
    }

    public Captain(Player player)
    {
        Player = player;
        CaptainManager.LivingCaptains.Add(this);
    }

}
