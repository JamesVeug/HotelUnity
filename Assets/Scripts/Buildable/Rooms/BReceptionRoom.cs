using System.Collections.Generic;
using UnityEngine;

public class BReceptionRoom : BuildableRoom{

    public BReceptionRoom() : this(0, 0, 0, 0)
    {

    }

    public BReceptionRoom(int x, int y, int width, int height) : base(x,y,width,height)
    {
    }

    public List<BuildableItem> placeableItems = new List<BuildableItem>
    {
        new BuildableReception(0,0)
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
