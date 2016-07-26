using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class IDoubleBed : BuildableBed
{
    private Gold cost = Gold.create(100);

    public override DRectangle Create(int x, int y)
    {
        // Double bed
        // xx
        // xx
        // xx

        width = 2;
        height = 3;

        // Tiles taken up
        tiles.Add(new Vector2(0, 0));
        tiles.Add(new Vector2(1, 0));
        tiles.Add(new Vector2(0, 1));
        tiles.Add(new Vector2(1, 1));
        tiles.Add(new Vector2(0, 2));
        tiles.Add(new Vector2(1, 2));

        // Item Tiles
        itemTiles.Add(new Vector2(0, 0));
        itemTiles.Add(new Vector2(1, 0));
        itemTiles.Add(new Vector2(0, 1));
        itemTiles.Add(new Vector2(1, 1));
        itemTiles.Add(new Vector2(0, 2));
        itemTiles.Add(new Vector2(1, 2));

        // Origin is the 5'th tile we create
        origin = tiles[4]; // TileVector to set when we want to indicate this item has changed

        return base.Create(x, y, 2, 3);
    }

    public override Vector2 getBedPosition(int bedSideIndex)
    {
        float angle = rotation.eulerAngles.y;
        if (angle == 0 && bedSideIndex == 0)   { return new Vector2(0 + left, 2 + top); }
        if (angle == 0 && bedSideIndex == 1)   { return new Vector2(1 + left, 2 + top); }
        if (angle == 90 && bedSideIndex == 0)  { return new Vector2(2 + left, 0 + top); }
        if (angle == 90 && bedSideIndex == 1)  { return new Vector2(2 + left, 1 + top); }
        if (angle == 180 && bedSideIndex == 0) { return new Vector2(-1 + left,-2 + top); }
        if (angle == 180 && bedSideIndex == 1) { return new Vector2(0 + left, -2 + top); }
        if (angle == 270 && bedSideIndex == 0) { return new Vector2(-2 + left, 0 + top); }
        if (angle == 270 && bedSideIndex == 1) { return new Vector2(-2 + left, 1 + top); }
        else
        {
            return Vector2.zero;
        }
    }

    public override int getMaxBedPositions()
    {
        return 2;
    }

    public override Gold purphaseCost()
    {
        return cost;
    }
}
