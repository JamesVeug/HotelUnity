using UnityEngine;
using System.Collections;
using System;

public class GameData : MonoBehaviour {
    public DTileMap dTileMap;
    public TileMap graphicsMap;
    public Navigation navigation;
    public GameLogic gameLogic;

    void Start()
    {
        navigation = ScriptableObject.CreateInstance<Navigation>();
        navigation.initialize();
        gameLogic = GetComponent<GameLogic>();
        if (gameLogic == null)
        {
            gameLogic = gameObject.AddComponent<GameLogic>();
        }
    }

    public DTileMap createTileMap(int width, int height)
    {
        if (dTileMap != null)
        {
            dTileMap.destroy();
        }
        dTileMap = gameObject.AddComponent<DTileMap>();
        dTileMap.initialize(width, height);
        return dTileMap;
    }
}
