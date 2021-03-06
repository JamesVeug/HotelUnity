﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameLogic : MonoBehaviour {

    public int soldBeds = 0;
    public int rejectedPeople = 0;

    public Gold currentMoney;

    public float aiSpawnFrequency = 0.5f;
    public MaidAI maidAI;
    public ReceptionistAI receptionistAI;
    public GuestAI AIToSpawn;

    private bool initialized;
    private float spawnTime;

    private List<StaffAI> spawnedStaffAI = new List<StaffAI>();
    private List<GuestAI> spawnedAI = new List<GuestAI>();

    private GameData data;


	// Use this for initialization
	void Start () {
        name = "GameLogic";
        data = GameObject.FindObjectOfType<GameData>();
        spawnTime = Time.time + 1;
        currentMoney = Gold.create(1000);
        spawnedStaffAI = new List<StaffAI>();
        spawnedAI = new List<GuestAI>();

    }
	
	// Update is called once per frame
	void Update () {
        if( !initialized)
        {
            initialize();
        }
        

        if( (spawnTime+aiSpawnFrequency) < Time.time)
        {

            int spawnIndex = UnityEngine.Random.Range(0,data.navigation.AISpawnLocations.Count);
            Vector2 spawn = data.navigation.AISpawnLocations[spawnIndex];
            /*Vector2 target = data.navigation.AISpawnLocations[data.navigation.AISpawnLocations.Count- spawnIndex-1];*/

            GuestAI ai = Instantiate(AIToSpawn);
            ai.transform.position = new Vector3(spawn.x, 0, spawn.y);
            //ai.targetPosition = new Vector3(target.x, 0, target.y);
            
            spawnedAI.Add(ai);
            spawnTime = Time.time;
        }
    }

    public Gold getGold()
    {
        return currentMoney;
    }

    public void addGold(Gold gold)
    {
        this.currentMoney += gold;

        //Debug.Log("Adding Gold " + currentMoney);
    }

    public void suntractGold(Gold gold)
    {
        this.currentMoney -= gold;
    }

    public MaidAI addHouseKeeper(Vector3 position)
    {
        MaidAI ai = Instantiate(maidAI);
        ai.transform.position = new Vector3(position.x, 0, position.z);
        spawnedStaffAI.Add(ai);
        return ai;
    }

    public ReceptionistAI addReceptionist(Vector3 position)
    {
        ReceptionistAI ai = Instantiate(receptionistAI);
        ai.transform.position = new Vector3(position.x, 0, position.z);
        spawnedStaffAI.Add(ai);
        return ai;
    }

    public void initialize()
    {
        int tilemapWidth = data.dTileMap.width;
        int tilemapHeight = data.dTileMap.height;

        // Setup pathway
        for (int i = 0; i < tilemapWidth; i++)
        {
            for (int j = 0; j < tilemapHeight; j++)
            {
                if (i < 2 || i > tilemapWidth-3 || j < 2 || j > tilemapHeight-3)
                {
                    data.dTileMap.setTile(i, j, DTile.TYPE_WALKWAY);
                }

            }
        }

        // Add AI
        float hts = data.graphicsMap.tileSize/2;
        data.navigation.AISpawnLocations.Add(new Vector2(0+ hts, hts));
        data.navigation.AISpawnLocations.Add(new Vector2(1 + hts, hts));
        data.navigation.AISpawnLocations.Add(new Vector2(0 + hts, tilemapHeight+hts-1));
        data.navigation.AISpawnLocations.Add(new Vector2(1 + hts, tilemapHeight + hts-1));

        data.navigation.AISpawnLocations.Add(new Vector2(tilemapWidth - 1 + hts, hts));
        data.navigation.AISpawnLocations.Add(new Vector2(tilemapWidth - 2 + hts, hts));
        data.navigation.AISpawnLocations.Add(new Vector2(tilemapWidth - 1 + hts, tilemapHeight + hts - 1));
        data.navigation.AISpawnLocations.Add(new Vector2(tilemapWidth - 2 + hts, tilemapHeight + hts - 1));

        //Debug.Log("Initialized");
        initialized = true;


        // Staff

        // Staff
        // Maid 19,21
        addHouseKeeper(new Vector3(19, 0, 21));
    }

    public BBedroom getDirtyRoom()
    {
        List<BBedroom> rooms = data.dTileMap.getBedrooms();

        BBedroom available = null;
        foreach (BBedroom r in rooms)
        {
            if (r.hasDirtyBeds())
            {

                available = r;
                break;
            }
        }

        if (available == null)
        {
            //Debug.Log("No Dirty Rooms available");
            return null;
        }

        // Return what we bought
        return available;
    }

    public BBedroom getAvailableRoom()
    {
        List<BBedroom> rooms = data.dTileMap.getBedrooms();

        BBedroom available = null;
        foreach(BBedroom r in rooms)
        {
            if (r.hasBedsAvailable())
            {

                available = r;
                break;
            }
        }

        if( available == null)
        {
            //Debug.Log("No Clean Rooms available");
            return null;
        }

        // Return what we bought
        return available;
    }
}
