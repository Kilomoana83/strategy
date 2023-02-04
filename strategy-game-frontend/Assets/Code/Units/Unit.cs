using UnityEngine;
using System.Collections.Generic;

public delegate void UnitReachedTargetField(Vector2Int oldPos, Unit unit);
public delegate void UnitTargetsNewGridField(Vector2Int newPos, Unit unit);
public delegate void UnitDestroyed(Unit unit);
public delegate void UnitIdles(Unit unit);

public class Unit : MonoBehaviour
{
    public event UnitReachedTargetField UnitReachedTargetField;
    public event UnitTargetsNewGridField UnitTargetsNewGridField;
    public event UnitDestroyed UnitDestroyed;
    public event UnitIdles UnitIdles;

    public Queue<Vector2Int> GridPath;
    public Vector2Int CurrentGridPos;
    public Vector2Int CurrentGridTarget;
    public Vector3 CurrentPosTarget;

    public Vector3 bounds;
    public UnitSO UnitSO;

    public Building WorkOrFightTargetBuilding;
    public Unit WorkOrFightTargetUnit;

    public PlayerType Player;

    public Grid m_grid;

    public int WoodInventory;
    public int Health;

    public bool hasWorkOrFights = false;

    public float lastFightTick;
    public float lastSearchTick;

    private void Start()
    {
        CurrentPosTarget = this.transform.position;
        Health = UnitSO.HitPoints;
    }

    public void Inject(Grid grid)
    {
        m_grid = grid;
    }

    protected virtual void Update()
    {
        int speed = Player == PlayerType.Player ? UnitSO.AbilityPlayer.Speed : UnitSO.AbilityEnemy.Speed;
        int damage = Player == PlayerType.Player ? UnitSO.AbilityPlayer.Damage : UnitSO.AbilityEnemy.Damage;

        if (Time.time - 0.01f > lastSearchTick)
        {
            lastSearchTick = Time.time;
            SearchForValidTarget();
        }

        if(WorkOrFightTargetUnit != null)
        {
            if (Time.time - lastFightTick > speed)
            {
                lastFightTick = Time.time;
                WorkOrFightTargetUnit.Attacked(damage, this);
            }
        }
        else if(WorkOrFightTargetBuilding != null)
        {
            if(Time.time - lastFightTick > speed)
            {
                lastFightTick = Time.time;
                WorkOrFightTargetBuilding.Attacked(damage, this);
            }
        }
        else if(CurrentPosTarget != this.transform.position)
        {
            Move();
        }
        else if(GridPath != null && GridPath.Count > 0)
        {
            CurrentGridPos = CurrentGridTarget;
            UnitReachedTargetField(CurrentGridTarget, this);

            CurrentGridTarget = GridPath.Dequeue();

            UnitTargetsNewGridField(CurrentGridTarget, this);

            CurrentPosTarget = CurrentGridTarget.FromGridPosToWorldPos(bounds);
        }
        else if(GridPath != null && GridPath.Count == 0)
        {
            CurrentGridPos = CurrentGridTarget;
            UnitIdles(this);
        }
    }

    public void Attacked(int strength, Unit unit)
    {
        Health -= strength;
        if(Health <= 0)
        {
            UnitDestroyed(this);
        }
    }

    public void SearchForValidTarget()
    {
        PlayerType player = Player == PlayerType.Player ? PlayerType.Enemy : PlayerType.Player;
        int range = Player == PlayerType.Player ? UnitSO.AbilityPlayer.Range : UnitSO.AbilityEnemy.Range;

        TargetInfo target = m_grid.SearchForTargetInRange(range, CurrentGridPos, player);
        if (target.building == null && target.unit == null) return;
        if (target.unit != null) WorkOrFightTargetUnit = target.unit;
        if (target.building != null) WorkOrFightTargetBuilding = target.building;
    }

    public void AssignWorkOrFightTargetBuilding(Building building)
    {
        WorkOrFightTargetBuilding = building;
    }

    public void AssignWorkOrFightTargetUnit(Unit unit)
    {
        WorkOrFightTargetUnit = unit;
    }

    private void Move()
    {
        int speed = Player == PlayerType.Player ? UnitSO.AbilityPlayer.Speed : UnitSO.AbilityEnemy.Speed;
        this.transform.position = Vector2.MoveTowards(this.transform.position, CurrentPosTarget, speed * Time.deltaTime);
    }
}
