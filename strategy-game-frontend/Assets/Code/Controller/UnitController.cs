using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void OnRessourcedEarned(int wood, int gold); 

public class UnitController : MonoBehaviour
{
    public event OnRessourcedEarned RessourcesEarned;
    public Transform Units;

    private Grid m_grid;
    private Vector3 m_boundsSize;

    public void Inject(Grid grid, Vector3 boundsSize)
    {
        m_grid = grid;
        m_boundsSize = boundsSize;
    }

    public void SpawnFighterUnit(PlayerType playerType, UnitSO unitSO, Vector2Int coords)
    {
        FighterUnit unit = UnitFactory.getGameObjectForFighter(m_grid, unitSO, playerType);
        unit.bounds = m_boundsSize;
        unit.transform.parent = Units;
        unit.gameObject.transform.localPosition = new Vector2(coords.x * m_boundsSize.x, coords.y * m_boundsSize.y);

        PlayerType player = playerType == PlayerType.Player ? PlayerType.Enemy : PlayerType.Player;
        Vector2Int enemyBuilding = m_grid.GetEnemyBuilding(player);

        if (enemyBuilding.x == -1)
        {
            Destroy(unit.gameObject);
            return;
        }

        Queue<Vector2Int> path = GeneratePathToTarget(coords, enemyBuilding);
        unit.GridPath = path;

        unit.UnitTargetsNewGridField += Unit_UnitTargetsNewGridField;
        unit.UnitReachedTargetField += Unit_UnitReachedTargetField;
        unit.UnitDestroyed += Unit_UnitDestroyed;
        unit.UnitIdles += Unit_UnitIdles;
    }

    public void SpawnWorkerUnit(PlayerType playerType, UnitSO unitSO, Vector2Int coords)
    {
        WorkerUnit unit = UnitFactory.getGameObjectForWorker(m_grid, unitSO, playerType);
        unit.bounds = m_boundsSize;
        unit.transform.parent = Units;
        unit.gameObject.transform.localPosition = new Vector2(coords.x * m_boundsSize.x, coords.y * m_boundsSize.y);
        Vector2Int freeTree = m_grid.GetFreeTree(PlayerType.Player, new Vector2Int(coords.x, coords.y));

        // see grid for further explanation; just killing a worker if the field is crowded or all trees are gone for now
        if(freeTree.x == -1)
        {
            Destroy(unit.gameObject);
            return;
        }

        Queue<Vector2Int> path = GeneratePathToTarget(new Vector2Int(coords.x, coords.y), freeTree);
        unit.GridPath = path;

        unit.UnitTargetsNewGridField += Unit_UnitTargetsNewGridField;
        unit.UnitReachedTargetField += Unit_UnitReachedTargetField;
        unit.UnitDestroyed += Unit_UnitDestroyed;
        unit.UnitIdles += Unit_UnitIdles;
    }

    private void Unit_UnitIdles(Unit unit)
    {
        if(unit.GetType() == typeof(WorkerUnit))
        {
            Vector2Int currentPos = unit.CurrentGridPos;
            if (unit.WoodInventory > 0 && unit.CurrentGridPos != GetPlayerBasePos(unit.Player))
            {
                Queue<Vector2Int> path = GeneratePathToTarget(unit.CurrentGridPos, GetPlayerBasePos(unit.Player));
                unit.GridPath = path;
            }
            // tree has already been harvested before target has been reached
            else if(unit.WoodInventory == 0 && unit.CurrentGridPos != GetPlayerBasePos(unit.Player) && m_grid.GetPlaceable(unit.CurrentGridPos.x, unit.CurrentGridPos.y) == null && m_grid.GetFreeTree(unit.Player, currentPos) != null)
            {
                Vector2Int freeTree = m_grid.GetFreeTree(unit.Player, currentPos);
                if (freeTree == null) return;

                Queue<Vector2Int> path = GeneratePathToTarget(currentPos, freeTree);
                unit.GridPath = path;
            }
            // nothing to do anymore - die
            else if(m_grid.GetFreeTree(unit.Player, currentPos) == null)
            {
                m_grid.RemoveUnit(unit.CurrentGridPos, unit);
                m_grid.RemoveUnit(unit.CurrentGridTarget, unit);
                Destroy(unit.gameObject);
            }
            else if(unit.WoodInventory > 0 && unit.CurrentGridPos == GetPlayerBasePos(unit.Player))
            {
                // if the enemy would have a wallet, we would grant the AI some wood here
                if(unit.Player == PlayerType.Player) RessourcesEarned(unit.WoodInventory, 0);
                unit.WoodInventory = 0;
                Vector2Int freeTree = m_grid.GetFreeTree(unit.Player, currentPos);
                Queue<Vector2Int> path = GeneratePathToTarget(currentPos, freeTree);
                unit.GridPath = path;
            }
            else
            {
                Building building = m_grid.GetPlaceable(unit.CurrentGridPos.x, unit.CurrentGridPos.y);
                unit.AssignWorkOrFightTargetBuilding(building);
            }
        }
        else
        {
            m_grid.RemoveUnit(unit.CurrentGridPos, unit);
            m_grid.RemoveUnit(unit.CurrentGridTarget, unit);
            Destroy(unit.gameObject);
        }
    }

    private Vector2Int GetPlayerBasePos(PlayerType player)
    {
        int y = m_grid.m_height / 2;
        if (player == PlayerType.Player) return new Vector2Int(2, y);
        else return new Vector2Int(m_grid.m_width - 3, y);
    }

    private void Unit_UnitDestroyed(Unit unit)
    {
        if(unit.Player == PlayerType.Enemy)
        {
            RessourcesEarned(0, unit.UnitSO.GoldRewardPerDeath);
        }

        m_grid.RemoveUnit(unit.CurrentGridPos, unit);
        m_grid.RemoveUnit(unit.CurrentGridTarget, unit);

        Destroy(unit.gameObject);
    }

    private void Unit_UnitReachedTargetField(Vector2Int oldPos, Unit unit)
    {
        m_grid.RemoveUnit(oldPos, unit);
    }

    private void Unit_UnitTargetsNewGridField(Vector2Int newPos, Unit unit)
    {
        m_grid.AddUnit(newPos, unit);
    }

    private Queue<Vector2Int> GeneratePathToTarget(Vector2Int start, Vector2Int goal)
    {
        PriorityQueue<Vector2Int, int> priorityQueue = new PriorityQueue<Vector2Int, int>(0);
        priorityQueue.Insert(start, 0);

        Dictionary<Vector2Int, Vector2Int> comesFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costsSoFar = new Dictionary<Vector2Int, int>();

        comesFrom.Add(start, Vector2Int.zero);
        costsSoFar.Add(start, 0);

        while(!priorityQueue.IsEmpty())
        {
            Vector2Int current = priorityQueue.Pop();

            if (current == goal) break;

            List<Vector2Int> neighbours = m_grid.GetNeighboursInRange(current, 0);
            for (int i = 0; i < neighbours.Count; i++)
            {
                Vector2Int next = neighbours[i];
                int newCosts = costsSoFar[current] + 1;
                if(!costsSoFar.ContainsKey(next) || newCosts < costsSoFar[next])
                {
                    costsSoFar.Add(next, newCosts);
                    int priority = newCosts;
                    priorityQueue.Insert(next, priority);
                    comesFrom.Add(next, current);
                }
            }
        }
        return PreparePathForUnit(goal, start, comesFrom);
    }

    private Queue<Vector2Int> PreparePathForUnit(Vector2Int goal, Vector2Int start, Dictionary<Vector2Int, Vector2Int> comesFrom)
    {
        Vector2Int current = goal;
        Vector2Int before = current;

        Queue<Vector2Int> path = new Queue<Vector2Int>();
        path.Enqueue(goal);

        while(current != start)
        {
            if (!comesFrom.ContainsKey(current)) return null;

            current = comesFrom[current];
            path.Enqueue(current);
        }

        Queue<Vector2Int> reversedQueue = new Queue<Vector2Int>();
        foreach(Vector2Int item in path.Reverse())
        {
            reversedQueue.Enqueue(item);
        }
        return reversedQueue;
    }
}
