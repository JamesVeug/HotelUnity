using UnityEngine;
using System.Collections;
using System;

public class GameData : MonoBehaviour {
    public DTileMap dTileMap;
    public TileMap graphicsMap;

    public DTileMap createTileMap(int width, int height)
    {
        if (dTileMap != null)
        {
            dTileMap.destroy();
        }
        dTileMap = new DTileMap(width, height);
        return dTileMap;
    }
}
