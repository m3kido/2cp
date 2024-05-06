using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDisposition", menuName = "Unit Disposition")]
public class UnitDispositionSO : ScriptableObject
{
    public UnitPlacement[] UnitPlacements;
}

[Serializable]
public struct UnitPlacement
{
    public Vector3Int Position;
    public EUnits Unit;
    public int Owner;
}