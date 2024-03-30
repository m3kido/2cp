using System;
using System.Collections.Generic;

[Serializable]
public class UnitManagerData
{
    public List<AttackingUnitSaveData> AttackingUnits;
    public List<LoadingUnitSaveData> LoadingUnits;

    public UnitManagerData(List<AttackingUnitSaveData> attakingUnits, List<LoadingUnitSaveData> loadingUnits)
    {
        AttackingUnits = attakingUnits;
        LoadingUnits = loadingUnits;
    }


}
