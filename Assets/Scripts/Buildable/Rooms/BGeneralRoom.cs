using System.Collections.Generic;
using UnityEngine;

public class BGeneralRoom : BuildableRoom{

    public BGeneralRoom() : this(0, 0, 0, 0)
    {

    }

    public BGeneralRoom(int x, int y, int width, int height) : base(x,y,width,height)
    {
    }

    public List<BuildableItem> placeableItems = new List<BuildableItem>
    {
        new BuildableChair(0,0),
        new BuildablePainting(0,0)
    };

    public override bool canBeBuilt()
    {
        return selectionScript.isValid();
    }

    public override List<BuildableItem> getPlaceableItems()
    {
        return placeableItems;
    }
}
