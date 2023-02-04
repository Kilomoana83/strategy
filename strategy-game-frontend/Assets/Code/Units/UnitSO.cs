using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "StrategyGame/Create Unit", order = 2)]
public class UnitSO : ScriptableObject
{
    public AbilitySO AbilityPlayer;
    public AbilitySO AbilityEnemy;
    public Sprite AssetPlayer;
    public Sprite AssetEnemy;
    public int HitPoints;
    public int GoldCosts;
    public int GoldRewardPerDeath;
}
