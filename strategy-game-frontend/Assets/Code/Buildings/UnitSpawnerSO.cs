using UnityEngine;

[CreateAssetMenu(fileName = "UnitSpawner", menuName = "StrategyGame/Create Unit Spawner", order = 4)]
public class UnitSpawnerSO : ScriptableObject
{
    public int CooldownInSeconds;
    public UnitSO Unit;
}
