using UnityEngine;

public class DItem
{
    public Vector2 position;
    public GameObject gameObject;

    public DItem(int x, int y)
    {
        position = new Vector2(x, y);
    }

    public DItem(Vector2 position)
    {
        this.position = new Vector2(position.x, position.y);
    }
}
