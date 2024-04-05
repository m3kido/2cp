using UnityEngine;

public class Captain 
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

    public Captain(CaptainData data) 
    {
        Data = data;
    }

}
