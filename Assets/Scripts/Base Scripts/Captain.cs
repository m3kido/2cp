using UnityEngine;

public abstract class Captain
{
    public CaptainData Data;
    public bool IsCelesteActive = false;
    public Player Player;


    public virtual void ApplySpecial()
    {

    }

    public virtual bool IsCelesteReady()
    {
        return false;
    }

}
