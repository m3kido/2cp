public class Captain
{
    public CaptainDataSO Data;
    public Player Player;
    public bool IsCelesteActive = false;



    public virtual void EnableCeleste()
    {
        
    }

    public virtual void DisableCeleste()
    {

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
