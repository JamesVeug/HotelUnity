﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ISingleBed : BuildableBed{

    public override DRectangle Create(int x, int y)
    {
        // Double bed
        // x
        // x

        // Tiles taken up
        tiles.Add(new Vector2(0, 0));
        tiles.Add(new Vector2(0, 1));

        // Item Tiles
        itemTiles.Add(new Vector2(0, 0));
        itemTiles.Add(new Vector2(0, 1));


        // Origin is the 5'th tile we create
        origin = tiles[0]; // TileVector to set when we want to indicate this item has changed


        return base.Create(x, y, 1, 2);
    }

    public override Vector2 getBedPosition(int bedSideIndex)
    {
        float angle = rotation.eulerAngles.y;
        if (angle == 0 && bedSideIndex == 0)   { return new Vector2(0 + left, 1 + top); }
        if (angle == 90 && bedSideIndex == 0)  { return new Vector2(1 + left, 0 + top); }
        if (angle == 180 && bedSideIndex == 0) { return new Vector2(0 + left,-1 + top); }
        if (angle == 270 && bedSideIndex == 0) { return new Vector2(-1 + left, 0 + top); }
        else
        {
            return Vector2.zero;
        }
    }

    public override int getMaxBedPositions()
    {
        return 1;
    }
}
