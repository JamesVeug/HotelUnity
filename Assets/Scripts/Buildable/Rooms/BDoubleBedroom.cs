using System.Collections.Generic;
using UnityEngine;

public class BDoubleBedroom : BuildableRoom{

    public BDoubleBedroom() : this(0, 0, 0, 0)
    {

    }

    public BDoubleBedroom(int x, int y, int width, int height) : base(x,y,width,height)
    {

    }

    public List<BuildableItem> placeableItems = new List<BuildableItem>
    {
        new BuildableMinibar(0,0),
        new BuildableBed(0,0)
    };

    public override bool canBeBuilt()
    {
        return doors.Count > 0 && selectionScript.isValid();
    }

    public override List<BuildableItem> getPlaceableItems()
    {
        return placeableItems;
    }
}
