using UnityEngine;

public delegate void MousePressedOnTile(Tile tile);

public class Tile : MonoBehaviour
{
    public event MousePressedOnTile OnMousePressedOnTile;

    public TileSO TileSO;
    public Vector2Int Pos;

    public void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnMousePressedOnTile(this);
        }
    }
}
