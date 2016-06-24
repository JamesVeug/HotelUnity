using UnityEngine;
using System.Collections;

public class BuildableBed : BuildableItem{

    public BuildableBed(int x, int y): base(x, y)
    {
        // Double bed
        // xx
        // xx
        // xx

        width = 2;
        height = 3;
        tiles.Add(new Vector2(0, 0));
        tiles.Add(new Vector2(1, 0));
        tiles.Add(new Vector2(0, 1));
        tiles.Add(new Vector2(1, 1));
        tiles.Add(new Vector2(0, 2));
        tiles.Add(new Vector2(1, 2));
        origin = tiles[4]; // TileVector to set when we want to indicate this item has changed
    }
}
