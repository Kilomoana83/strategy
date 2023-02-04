using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private Tile[] m_grid;
    private Building[] m_placeables;
    private List<Unit>[] m_units;

    public int m_width;
    public int m_height;

    public Grid(TileSO defaultTile, int height, int width)
    {
        m_width = width;
        m_height = height;
        m_grid = new Tile[m_height * m_width];
        m_placeables = new Building[m_height * m_width];
        m_units = new List<Unit>[m_height * m_width];
        PrepareUnitsGrid();
    }

    private void PrepareUnitsGrid()
    {
        for(int i = 0; i < m_units.Length; i++)
        {
            m_units[i] = new List<Unit>();
        }
    }

    public int OneDimensionalIndexFromCoords(int x, int y)
    {
        return x + m_width * y;
    }

    public Tile GetTile(int x, int y)
    {
        return m_grid[OneDimensionalIndexFromCoords(x, y)];
    }

    public Building GetPlaceable(int x, int y)
    {
        return m_placeables[OneDimensionalIndexFromCoords(x, y)];
    }

    public Building GetPlaceable(int index)
    {
        return m_placeables[index];
    }

    public void SetTileForCoords(int x, int y, Tile tile)
    {
        m_grid[OneDimensionalIndexFromCoords(x, y)] = tile;
    }

    public void SetPlaceableForCoords(int x, int y, Building tile)
    {
        m_placeables[OneDimensionalIndexFromCoords(x, y)] = tile;
    }

    public void AddUnit(Vector2Int coords, Unit unit)
    {
        m_units[OneDimensionalIndexFromCoords(coords.x, coords.y)].Add(unit);
    }

    public void RemoveUnit(Vector2Int coords, Unit unit)
    {
        m_units[OneDimensionalIndexFromCoords(coords.x, coords.y)].Remove(unit);
    }

    public List<Vector2Int> GetNeighboursInRange(Vector2Int coords, int range)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        if(coords.x - range > 0) neighbours.Add(new Vector2Int(coords.x - 1 - range, coords.y));
        if(coords.x + range < m_width - 1) neighbours.Add(new Vector2Int(coords.x + 1 + range, coords.y));
        if (coords.y + range < m_height - 1) neighbours.Add(new Vector2Int(coords.x, coords.y + 1 + range));
        if (coords.y - range > 0) neighbours.Add(new Vector2Int(coords.x, coords.y - 1 - range));

        return neighbours;
    }

    public Vector2Int GetFreeTree(PlayerType playerType, Vector2Int currentPosition) {

        // to keep it simple; as we know, the worker will spawn left of the players base
        // to prevent cutting trees of the enemy (which later might be even interesting)
        // we just prevent it with setting a max distance to half of the game field (-1)
        int maxDistance = m_width / 2 - 1;

        // using a vector 3 here to just store the distance in the z axis
        Vector3Int closestTree = new Vector3Int(-1,-1,9999);
        
        // to have a nicer look and feel on the map, I want workers to spread a bit out if there is already someone
        // on a tree; due to the simple implementation, it could happen that more workers go to the first tree immediately
        // if we reach 10 workers per existing tree (trees can be taken down so thats a valid scenario) the workers won't do
        // anything and just die (boring if there is nothing to do ;))
        int workersThreshold = 0;

        while (closestTree.z == 9999 && workersThreshold < 10)
        {
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    int i = OneDimensionalIndexFromCoords(x, y);
                    if (m_placeables[i] != null && m_placeables[i].BuildingSO.TileType == TileType.tree && m_units[i].Count <= workersThreshold)
                    {
                        int treeDistance = currentPosition.ManhattanDistance(new Vector2Int(x, y));
                        if (treeDistance < closestTree.z && treeDistance < maxDistance)
                        {
                            closestTree = new Vector3Int(x, y, treeDistance);
                        }
                    }
                }
            }
            workersThreshold++;
        }
        
        return new Vector2Int(closestTree.x, closestTree.y);
    }

    public Vector2Int GetEnemyBuilding(PlayerType playerType)
    {
        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                int i = OneDimensionalIndexFromCoords(x, y);
                if (m_placeables[i] != null && m_placeables[i].BuildingSO.TileType != TileType.tree && m_placeables[i].Player == playerType)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1,-1);
    }

    public Vector2Int GetGridPositionOfTile(Tile tile)
    {
        for(int x = 0; x < m_width; x++)
        {
            for(int y = 0; y < m_height; y++)
            {
                if(m_grid[OneDimensionalIndexFromCoords(x,y)])
                {
                    return new Vector2Int(x,y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public TargetInfo SearchForTargetInRange(int range, Vector2Int pos, PlayerType player)
    {
        for(int i = 0; i < range + 1; i++)
        {
            List<Vector2Int> positions = GetNeighboursInRange(pos, range);
            for(int j = 0; j < positions.Count; j++)
            {
                List<Unit> units = m_units[OneDimensionalIndexFromCoords(positions[j].x, positions[j].y)];
                for(int k = 0; k < units.Count; k++)
                {
                    if(units[k].Player == player)
                    {
                        TargetInfo targetInfo = new TargetInfo();
                        targetInfo.unit = units[k];
                        targetInfo.pos = positions[i];
                        return targetInfo;
                    }
                }

                Building building = m_placeables[OneDimensionalIndexFromCoords(positions[j].x, positions[j].y)];

                if(building != null && building.Player == player)
                {
                    TargetInfo targetInfo = new TargetInfo();
                    targetInfo.building = building;
                    targetInfo.pos = positions[i];
                    return targetInfo;
                }
            }
        }
        return new TargetInfo();
    }
}
