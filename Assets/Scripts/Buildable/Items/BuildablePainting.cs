using UnityEngine;
using System.Collections;

public class BuildablePainting : BuildableItem{

    public override DRectangle Create(int x, int y)
    {
        tiles.Add(new Vector2(0,0));
        itemTiles.Add(new Vector2(0, 0));
        origin = tiles[0]; // TileVector to set when we want to indicate this item has changed

        return base.Create(x, y, 1, 1);
    }
}
