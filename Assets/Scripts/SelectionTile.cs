using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SelectionTile : MonoBehaviour {
    public GameObject tileObject;

    public Material selectionFailMaterial;
    public Material selectionWorkMaterial;

    [SerializeField]
    private Buildable builder;
    private int builderStage = 0;
    private string builderProperty = "";
    private bool builderIsModified = false;
    private bool buildStageChanged = false;
    private bool buildPropertyChanged = false;
    private bool buildDoorsChanged = false;
    private bool buildWindowsChanged = false;
    private bool buildWallsChanged = false;
    private bool placedItemsChanged = false;
    public List<DDoor> doors = new List<DDoor>();
    public List<DWindow> windows = new List<DWindow>();
    public List<DWall> walls = new List<DWall>();
    public List<BuildableItem> placedItems = new List<BuildableItem>();

    public DRectangle rect;
    private GameData data;
    private List<Renderer> rend;
    private bool valid;
    private List<GameObject> tilesSelectionObjects = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        rend = new List<Renderer>();// tileObject.GetComponent<Renderer>();
        data = FindObjectOfType<GameData>();
        rect = ScriptableObject.CreateInstance<DRectangle>().Create(0,0,1,1);
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {

        // Move Modified into this method
        // This is not being called before DTileMap!!!
        builderIsModified = calculationModified();


        /*Vector3 asignedPosition = new Vector3(
            Mathf.Clamp(rect.left, 0, data.dTileMap.width - rect.left),
            transform.position.y,
            Mathf.Clamp(rect.top, 0, data.dTileMap.height - rect.top)
        );*/
        transform.position = new Vector3(rect.left,0,rect.top);
        buildGraphics();

		if (builder is BuildableRoom) {

			// Setup for Building a room
			if (builderStage == BuildableRoom.STAGE_ITEMS) {
				setupSelectionTileForItems ();
			} else {
				setupSelectionTileForBuildingARoom ();
			}
		}
		else if (builder is BuildableStaff)
		{
			setupSelectionTileForStaff();
		}
    }

	public void setupSelectionTileForStaff()
	{
		int x = rect.left;
		int y = rect.top;

		// Needs more options!
		if (data.dTileMap.isRoom(x,y) || data.dTileMap.isWalkWay(x,y))
		{
			valid = true;
		}
		else {
			valid = false;
		}

		// Render the position
		for (int i = 0; i < rend.Count; i++)
		{
			Renderer r = rend[i];
			//Debug.Log("Objects " + i + "/" + tilesSelectionObjects.Count);
			//Debug.Log("Rend " + i + "/" + rend.Count);
			if (valid)
			{
				r.material = selectionWorkMaterial;
			}
			else
			{
				r.material = selectionFailMaterial;
			}
			r.material.mainTextureScale = new Vector2(rect.width, rect.height);
			tilesSelectionObjects[i].transform.localPosition = Vector3.zero;
			tilesSelectionObjects[i].transform.localScale = rect.size;
			//Debug.Log("Position " + tilesSelectionObjects[i].transform.localPosition);

		}

	}

    public void setupSelectionTileForItems()
    {
        // Placing items. Check We are placing it in the current room
        BuildableRoom room = (BuildableRoom)builder;
        BuildableItem placingItem = room.getCurrentBuildingItem();
        rect.width = (int)placingItem.width;
        rect.height = (int)placingItem.height;

        // Valid by default. Prove it wrong!
        valid = true;

        // Render the position
        List<Vector2> tiles = placingItem.getTiles();
        //Debug.Log("Tiles " + tiles.Count);

        DRectangle temp = ScriptableObject.CreateInstance<DRectangle>();
        for (int i = 0; i < rend.Count; i++)
        {
            Renderer r = rend[i];
            GameObject o = tilesSelectionObjects[i];

            //Debug.Log(tilesSelectionObjects[i]);
            //Debug.Log(tiles[i]);
            float x = tiles[i].x - rect.width  / 2 - placingItem.left;
            float z = tiles[i].y - rect.height / 2 - placingItem.top;

            o.transform.localPosition = new Vector3(x,0,z);
            o.transform.localScale = Vector3.one;


            temp.Create(o.transform.position, o.transform.localScale);
            if (room.contains(temp) && canHaveItemsBuiltOn(temp))
            {
                r.material = selectionWorkMaterial;
            }
            else
            {
                valid = false;
                r.material = selectionFailMaterial;
            }
            r.material.mainTextureScale = Vector2.one;
        }
    }

    public void setupSelectionTileForBuildingARoom()
    {
        //Debug.Log(rect.ToString());


        //DRectangle floorRect = rect.collapse(1, 1);
        if (rect.width < 5 || rect.height < 5 || data.dTileMap.collides(rect) || !canHaveRoomBuiltOn(rect))
        {
            valid = false;
        }
        else {
            valid = true;
        }



        // Render the position
        for (int i = 0; i < rend.Count; i++)
        {
            Renderer r = rend[i];
            //Debug.Log("Objects " + i + "/" + tilesSelectionObjects.Count);
            //Debug.Log("Rend " + i + "/" + rend.Count);
            if (valid)
            {
                r.material = selectionWorkMaterial;
            }
            else
            {
                r.material = selectionFailMaterial;
            }
            r.material.mainTextureScale = new Vector2(rect.width, rect.height);
            tilesSelectionObjects[i].transform.localPosition = Vector3.zero;
            tilesSelectionObjects[i].transform.localScale = rect.size;
            //Debug.Log("Position " + tilesSelectionObjects[i].transform.localPosition);

        }
    }

    private void buildGraphics()
    {
        int tileRequirements = 0;
        if (builderStage == BuildableRoom.STAGE_ITEMS)
        {
            BuildableRoom room = (BuildableRoom)builder;
            List<Vector2> tiles = room.getCurrentBuildingItem().getTiles();
            tileRequirements = tiles.Count;
        }
        else
        {

            tileRequirements = 1;
        }

        // Add extra tiles
        while (rend.Count < tileRequirements)
        {
            GameObject o = Instantiate(tileObject);
            o.transform.parent = gameObject.transform;
            o.transform.localPosition = Vector3.zero;

            GameObject child = o.transform.gameObject;
            tilesSelectionObjects.Add(child);


            Renderer r = child.transform.GetChild(0).GetComponent<Renderer>();
            rend.Add(r);
        }

        // Remove unneeded tiles
        while (rend.Count > tileRequirements)
        {
            //Debug.Log("Removing one with size " + rend.Count + " children: " + transform.childCount);
            GameObject o = tilesSelectionObjects[tilesSelectionObjects.Count - 1];
            tilesSelectionObjects.RemoveAt(tilesSelectionObjects.Count - 1);
            Destroy(o);

            rend.RemoveAt(rend.Count - 1);
        }
        //Debug.Log("Final size " + rend.Count);

    }

    private bool canHaveItemsBuiltOn(DRectangle rect)
    {
        for (int x = rect.left; x <= rect.right; x++)
        {
            for (int y = rect.top; y <= rect.bottom; y++)
            {
                // False if it's already an item
                if (data.dTileMap.hasItem(x, y)) return false;

                if (data.dTileMap.isRoom(x, y)) continue;
                if (data.dTileMap.hasWall(x, y)) continue;
                return false;
            }
        }

        return true;
    }

    private bool canHaveRoomBuiltOn(DRectangle rect)
    {
        for (int x = rect.left; x <= rect.right; x++)
        {
            for (int y = rect.top; y <= rect.bottom; y++)
            {
                if (!data.dTileMap.isGrass(x, y))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void setBuilder(Buildable b)
    {
        this.builder = b;
        this.builderStage = b.getStage();
        this.builderProperty = b.getProperty();
        doors.Clear();
        windows.Clear();
        walls.Clear();

        if (tilesSelectionObjects.Count > 0)
        {
            tilesSelectionObjects[0].transform.localScale = Vector3.one;
            tilesSelectionObjects[0].transform.localPosition = Vector3.zero;
        }
    }

    public Buildable getBuildable()
    {
        return builder;
    }

    void OnEnable()
    {
        Debug.Log("Map -> " + FindObjectOfType<TileMapMouse>());
    }

    public bool calculationModified()
    {
        // Record what has changed 
        //Debug.Log(builder);
        buildStageChanged = this.builderStage != builder.getStage();
        buildPropertyChanged = this.builderProperty != builder.getProperty();
        bool roomChanged = false;
        if (builder is BuildableRoom)
        {
            roomChanged = checkRoomChanged();
        }
        else if (builder is BuildableTile)
        {
            roomChanged = checkTileChanged();
        }

        if (buildPropertyChanged || buildStageChanged || roomChanged)
        {
            // Record changes and tell the graphics that we have changed
            this.builderStage = builder.getStage();
            this.builderProperty = builder.getProperty();
            return true;
        }

        // No Change
        return false;
    }

    public bool isModified()
    {
        return builderIsModified;
    }

    public bool checkTileChanged()
    {
        BuildableTile room = (BuildableTile)builder;
        bool changed = false;

        if( room.changedTile != Vector2.zero)
        {
            data.dTileMap.setTile((int)room.changedTile.x, (int)room.changedTile.y, room.changedValue);
            room.changedTile = Vector2.zero;
            changed = true;
        }

        return changed;
    }

    public bool checkRoomChanged()
    {
        BuildableRoom room = (BuildableRoom)builder;
        bool changed = false;

        // Doors
        if (!containsAll(this.doors, room.doors))
        {
            this.doors.Clear();
            this.doors.AddRange(room.doors);
            buildDoorsChanged = true;
            changed = true;
        }
        else
        {
            buildDoorsChanged = false;
        }


        // Windows
        if (!containsAll(this.windows, room.windows))
        {
            this.windows.Clear();
            this.windows.AddRange(room.windows);
            buildWindowsChanged = true;
            changed = true;
        }
        else
        {
            buildWindowsChanged = false;
        }

        // Walls
        if (!containsAll(this.walls, room.walls))
        {
            this.walls.Clear();
            this.walls.AddRange(room.walls);
            buildWallsChanged = true;
            changed = true;
        }
        else
        {
            buildWallsChanged = false;
        }

        // Placed items
        if (!containsAll(placedItems, room.items))
        {
            placedItemsChanged = true;
            placedItems.Clear();
            placedItems.AddRange(room.items);
            changed = true;
        }
        else
        {
            placedItemsChanged = false;
        }

        // Something changed
        return changed;
    }

    public bool stageModified()
    {
        return buildStageChanged;
    }

    public bool properyModified()
    {
        return buildPropertyChanged;
    }

    public bool doorsModified()
    {
        return buildDoorsChanged;
    }

    public bool windowsModified()
    {
        return buildWindowsChanged;
    }

    public bool wallsModified()
    {
        return buildWallsChanged;
    }

    public bool placedItemsModified()
    {
        return placedItemsChanged;
    }

    public bool isValid()
    {
        return valid;
    }

    private bool containsAll(List<DDoor> a, List<DDoor> b)
    {
        foreach (DDoor L in b)
        {
            if (!a.Contains(L))
            {
                return false;
            }
        }
        foreach (DDoor L in a)
        {
            if (!b.Contains(L))
            {
                return false;
            }
        }

        return true;
    }

    private bool containsAll(List<DWindow> a, List<DWindow> b)
    {
        foreach (DWindow L in b)
        {
            if (!a.Contains(L))
            {
                return false;
            }
        }
        foreach (DWindow L in a)
        {
            if (!b.Contains(L))
            {
                return false;
            }
        }

        return true;
    }

    private bool containsAll(List<DWall> a, List<DWall> b)
    {
        foreach (DWall L in b)
        {
            if (!a.Contains(L))
            {
                return false;
            }
        }
        foreach (DWall L in a)
        {
            if (!b.Contains(L))
            {
                return false;
            }
        }

        return true;
    }

    private bool containsAll(List<BuildableItem> a, List<BuildableItem> b)
    {
        foreach (BuildableItem L in b)
        {
            if (!a.Contains(L))
            {
                return false;
            }
        }
        foreach (BuildableItem L in a)
        {
            if (!b.Contains(L))
            {
                return false;
            }
        }

        return true;
    }
}
