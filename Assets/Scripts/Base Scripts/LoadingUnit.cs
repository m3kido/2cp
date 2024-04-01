public class LoadingUnit : Unit
{
    public AttackingUnitSaveData LoadedUnit { get; set; }

    public LoadingUnitSaveData GetDataToSave()
    {
        return new LoadingUnitSaveData(UnitType, Health, Provisions, Owner, HasMoved, LoadedUnit, GetGridPosition());
    }

    public void SetSavedData(LoadingUnitSaveData saveData)
    {
        Health = saveData.Health;
        Provisions = saveData.Provisions;
        Owner = saveData.Owner;
        UnitType = saveData.UnitType;
        HasMoved = saveData.HasMoved;
        LoadedUnit = saveData.LoadedUnitSaveData;
    }
}
