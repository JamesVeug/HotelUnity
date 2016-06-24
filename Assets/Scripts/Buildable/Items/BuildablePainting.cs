using UnityEngine;
using System.Collections;

public class BuildablePainting : BuildableItem{

    public BuildablePainting(int x, int y): base(x, y)
    {
        tiles.Add(new Vector2(0,0));
        origin = tiles[0]; // TileVector to set when we want to indicate this item has changed
    }
}
