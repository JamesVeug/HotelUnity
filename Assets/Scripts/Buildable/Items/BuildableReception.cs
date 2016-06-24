using UnityEngine;
using System.Collections;

public class BuildableReception : BuildableItem{

    public BuildableReception(int x, int y): base(x, y)
    {
        // Reception Desk
        // -X-   /z\
        // XXX    |
        // -X-    |

        width = 3;
        height = 3;
        tiles.Add(new Vector2(1, 0));
        tiles.Add(new Vector2(0, 1));
        tiles.Add(new Vector2(1, 1));
        tiles.Add(new Vector2(2, 1));
        tiles.Add(new Vector2(1, 2));
        origin = tiles[1]; // TileVector to set when we want to indicate this item has changed
    }
}
