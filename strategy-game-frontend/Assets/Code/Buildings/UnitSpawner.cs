using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public UnitSpawnerSO UnitSpawnerSO;
    public PlayerType PlayerType;
    public Vector2Int SpawnPosition;
    public UnitController UnitController;
    public float LastSpawn;
    public bool Active;

    public void Inject(UnitController unitController)
    {
        UnitController = unitController;
    }

    private void Update()
    {
        if (!Active) return;
        if (UnitSpawnerSO == null) return;

        if(Time.time - LastSpawn > UnitSpawnerSO.CooldownInSeconds)
        {
            LastSpawn = Time.time;
            UnitController.SpawnFighterUnit(PlayerType, UnitSpawnerSO.Unit, SpawnPosition);
        }
    }
}
