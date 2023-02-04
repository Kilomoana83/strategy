using UnityEngine;

public static class Extensions
{
    public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public static Vector2 FromGridPosToWorldPos(this Vector2Int a, Vector3 bounds)
    {
        return new Vector2(a.x * bounds.x, a.y * bounds.y);
    }

    public static Vector2Int FromWorldPosToGridPos(this Vector3 a, Vector3 bounds)
    {
        return new Vector2Int((int)(a.x / bounds.x), (int)(a.y / bounds.y));
    }
}
