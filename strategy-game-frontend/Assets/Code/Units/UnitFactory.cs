using UnityEngine;

public static class UnitFactory
{
    public static FighterUnit getGameObjectForFighter(Grid grid, UnitSO unitSO, PlayerType player)
    {
        GameObject go = new GameObject();
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();

        if (player == PlayerType.Enemy) spriteRenderer.sprite = unitSO.AssetEnemy;
        else spriteRenderer.sprite = unitSO.AssetPlayer;
        spriteRenderer.sortingOrder = 10;

        FighterUnit unit = go.AddComponent<FighterUnit>();
        unit.UnitSO = unitSO;
        unit.Inject(grid);
        unit.Player = player;

        return unit;
    }

    public static WorkerUnit getGameObjectForWorker(Grid grid, UnitSO unitSO, PlayerType player)
    {
        GameObject go = new GameObject();
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();

        if (player == PlayerType.Enemy) spriteRenderer.sprite = unitSO.AssetEnemy;
        else spriteRenderer.sprite = unitSO.AssetPlayer;
        spriteRenderer.sortingOrder = 10;

        WorkerUnit unit = go.AddComponent<WorkerUnit>();
        unit.Inject(grid);
        unit.Player = player;
        unit.UnitSO = unitSO;
        
        return unit;
    }
}
