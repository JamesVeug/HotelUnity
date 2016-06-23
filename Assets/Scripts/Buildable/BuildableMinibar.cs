using UnityEngine;
using System.Collections;

public class BuildableMinibar : BuildableItem{

    public BuildableMinibar(int x, int y): base(x, y)
    {
        tiles.Add(new Vector2(0,0));
    }
}
