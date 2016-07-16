using UnityEngine;
using System.Collections;
using System;

public class BuildableReception : BuildableItem{

    //private Vector2 guestPosition = Vector2.zero;
    //private Vector2 staffPosition = Vector2.zero;

    public override DRectangle Create(int x, int y)
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


        // Item Tiles
        itemTiles.Add(new Vector2(0, 1));
        itemTiles.Add(new Vector2(1, 1));
        itemTiles.Add(new Vector2(2, 1));

        origin = new Vector2(1, 1); // TileVector to set when we want to indicate this item has changed
        
        return base.Create(x, y, 3, 3);
    }

    public Vector2 getGuestPosition()
    {
        float angle = rotation.eulerAngles.y;
        if ( angle == 0)
        {
            return new Vector2(1 + left, 2 + top);
        }
        else if (angle == 90)
        {
            return new Vector2(2 + left, 1 + top);
        }
        else if (angle == 180)
        {
            return new Vector2(1 + left, 0 + top);
        }
        else
        {
            return new Vector2(0 + left, 1 + top);
        }

    }

    public Vector2 getStaffPosition()
    {
        float angle = rotation.eulerAngles.y;
        if ( angle == 180)
        {
            return new Vector2(1 + left, 2 + top);
        }
        else if (angle == 270)
        {
            return new Vector2(2 + left, 1 + top);
        }
        else if (angle == 0)
        {
            return new Vector2(1 + left, 0 + top);
        }
        else
        {
            return new Vector2(0 + left, 1 + top);
        }
    }
}
