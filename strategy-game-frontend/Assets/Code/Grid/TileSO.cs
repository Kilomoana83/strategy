using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "StrategyGame/Create Tile", order = 5)]
public class TileSO : ScriptableObject
{
    public TileType TileType;
    public Sprite Asset;
}
