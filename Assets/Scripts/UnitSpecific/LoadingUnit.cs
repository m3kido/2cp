public class LoadingUnit : Unit
{
    private AttackingUnitSaveData _loadedUnit;

    public AttackingUnitSaveData LoadedUnit { get => _loadedUnit; set => _loadedUnit = value; }

    public LoadingUnitSaveData GetSaveData()
    {
        return new LoadingUnitSaveData(Health, Fuel, Owner, Type, HasMoved, LoadedUnit, GetGridPosition());
    }

    public void SetSaveData(LoadingUnitSaveData saveData)
    {
        Health = saveData.Health;
        Fuel = saveData.Fuel;
        Owner = saveData.Owner;
        Type = saveData.Type;
        HasMoved = saveData.HasMoved;
        LoadedUnit = saveData.LoadedUnitSaveData;

    }
}
