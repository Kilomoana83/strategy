using UnityEngine;

public class TileFactory
{
    public static Tile getTile(TileSO tileSO)
    {
        GameObject go = new GameObject();
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tileSO.Asset;
        Tile tile = go.AddComponent<Tile>();
        go.AddComponent<BoxCollider2D>();
        tile.TileSO = tileSO;
        return tile;
    }

    public static Building getBuilding(BuildingSO buildingSO, PlayerType player)
    {
        GameObject go = new GameObject();
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = buildingSO.Asset;

        Building building = go.AddComponent<Building>();
        building.BuildingSO = buildingSO;

        UnitSpawner spawner = go.AddComponent<UnitSpawner>();
        spawner.UnitSpawnerSO = building.BuildingSO.BasicUnitSpawner;
        spawner.PlayerType = player;

        building.UnitSpawners = new System.Collections.Generic.List<UnitSpawner>();
        building.UnitSpawners.Add(spawner);
        
        return building;
    }

}
