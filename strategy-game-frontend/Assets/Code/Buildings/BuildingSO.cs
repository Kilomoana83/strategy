using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "StrategyGame/Create Building", order = 3)]
public class BuildingSO : TileSO
{
    public int WoodCosts;
    public int HitPoints;
    public UnitSpawnerSO BasicUnitSpawner;
}
