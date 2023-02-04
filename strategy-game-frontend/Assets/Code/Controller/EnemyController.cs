using UnityEngine;

public delegate void OnWantToPlaceBarracks(int x, int y);

public class EnemyController : MonoBehaviour
{
    public event OnWantToPlaceBarracks OnPlaceBarracks;
    public int SecondsForBuildingAWorker = 10;
    public float LastWorkerBuild;

    public int SecondsForBuildingBarracks = 40;
    public float LastBarracksBuild;

    private UnitSO m_workerUnit;
    private Vector2Int m_workerSpawn;

    private UnitController m_unitController;
    private Grid m_grid;

    public void Inject(Grid grid, UnitController unitController, UnitSO workerUnit, Vector2Int workerSpawn)
    {
        m_unitController = unitController;
        m_workerUnit = workerUnit;
        m_workerSpawn = workerSpawn;
        m_grid = grid;
    }

    [System.Obsolete]
    void Update()
    {
        if(Time.time - LastWorkerBuild > SecondsForBuildingAWorker)
        {
            LastWorkerBuild = Time.time;
            m_unitController.SpawnWorkerUnit(PlayerType.Enemy, m_workerUnit, m_workerSpawn);
        }

        if (Time.time - LastBarracksBuild > SecondsForBuildingBarracks)
        {
            LastBarracksBuild = Time.time;

            int fromX = m_grid.m_width/2;
            int toX = m_grid.m_width - 1;
            int fromY = 0;
            int toY = m_grid.m_height - 1;

            int x = Random.RandomRange(fromX, toX);
            int y = Random.RandomRange(fromY, toY);

            while(m_grid.GetPlaceable(x, y) != null)
            {
                x = Random.RandomRange(fromX, toX);
                y = Random.RandomRange(fromY, toY);
            }
            OnPlaceBarracks(x, y);
        }
    }
}
