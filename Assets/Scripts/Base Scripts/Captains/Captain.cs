// Class to represent a captain
public abstract class Captain
{
    #region Fields
    public ECaptains CaptainName;
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
    public float maxSuperMeter;
    private float _superMeter;
    public float SuperMeter
    {
        get
        {
            return _superMeter;
        }

        set
        {
            if(value < 0)
            {
                _superMeter = 0;
            }
            else if(value > maxSuperMeter) {
                _superMeter = maxSuperMeter;
            }
            else
            {
                _superMeter = value; 
            }
        }
    }
    #endregion

    #region Methods
    public virtual void EnableCeleste()
    {

        IsCelesteActive = true;

    }

    public virtual void DisableCeleste()
    {

        IsCelesteActive = false;
    }

    public bool IsCelesteReady()
    {
        return (SuperMeter == maxSuperMeter);
    }

    public Captain(Player player)
    {
        Player = player;
        CaptainManager.LivingCaptains.Add(this);
    }

    public virtual void UnsubscribeWhenDestroyed() { }
    

    public CaptainSaveData GetDataToSave()
    {
        return new CaptainSaveData(CaptainName, IsCelesteActive, SuperMeter);
    }

    public void SetSaveData(CaptainSaveData captainData)
    {
        CaptainName = captainData.CaptainName;
        IsCelesteActive = captainData.IsCelesteActive;
        SuperMeter = captainData.SuperMeter;
    }
    #endregion
}
