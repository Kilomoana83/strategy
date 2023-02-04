using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "StrategyGame/Create Ability", order = 1)]
public class AbilitySO : ScriptableObject
{
    [SerializeField]
    private int m_range;
    [SerializeField]
    private int m_damage;
    [SerializeField]
    private int m_speed;

    private int range;
    private int damage;
    private int speed;

    public int Range { get { return range; } set { range = value; } }
    public int Damage { get { return damage; } set { damage = value; } }
    public int Speed { get { return speed; } set { speed = value; } }

    private void OnEnable()
    {
        range = m_range;
        damage = m_damage;
        speed = m_speed;
    }
}
