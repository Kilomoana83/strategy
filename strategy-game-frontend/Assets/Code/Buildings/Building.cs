using System.Collections.Generic;

public delegate void OnBuildingDestroyed(Building building, Unit byUnit);

public class Building : Tile
{
    public PlayerType Player;
    public event OnBuildingDestroyed BuildingDestroyed;
    public BuildingSO BuildingSO;
    public List<UnitSpawner> UnitSpawners;
    public int Health;

    private void Start()
    {
        UnitSpawners = new List<UnitSpawner>();
        Health = BuildingSO.HitPoints;
    }

    public void Attacked(int strength, Unit unit)
    {
        Health -= strength;
        if(Health <= 0)
        {
            unit.WoodInventory += 10;
            BuildingDestroyed(this, unit);
        }
    }
}