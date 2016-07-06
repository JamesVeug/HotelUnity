using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SelectionTile : MonoBehaviour {
    public GameObject tileObject;
    public Vector3 defaultTileObjectSize = new Vector3(1, 0.5f, 1);
    public Vector3 defulatTileObjectPosition = new Vector3(0.5f, 0.125f, 0.5f);

    public Material selectionFailMaterial;
    public Material selectionWorkMaterial;

    [SerializeField]
    private Buildable builder;
    private int builderStage = 0;
    private string builderProperty = "";
    private bool buildStageChanged = false;
    private bool buildPropertyChanged = false;
    private bool buildDoorsChanged = false;
    private bool buildWindowsChanged = false;
    private bool placedItemsChanged = false;
    private List<DDoor> doorPositions = new List<DDoor>();
    private List<DWindow> windowPositions = new List<DWindow>();
    private List<BuildableItem> placedItems = new List<BuildableItem>();

    public DRectangle rect;
    private GameData data;
    private Renderer rend;
    private bool valid;

    // Use this for initialization
    void Start ()
    {
        rend = tileObject.GetComponent<Renderer>();
        data = FindObjectOfType<GameData>();
        rect = ScriptableObject.CreateInstance<DRectangle>();
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        Vector3 asignedPosition = new Vector3(
            Mathf.Clamp(transform.position.x, 0, data.dTileMap.width - transform.localScale.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, 0, data.dTileMap.height - transform.localScale.z)
        );
        transform.position = asignedPosition;

        rect.left = (int)transform.position.x;
        rect.top = (int)transform.position.z;
        rect.width = (int)transform.localScale.x;
        rect.height = (int)transform.localScale.z;
        //Debug.DrawLine(new Vector3(rect.left, 0.5f, rect.top), new Vector3(rect.left, 0.5f, rect.top+rect.height));
        //Debug.DrawLine(new Vector3(rect.left, 0.5f, rect.top), new Vector3(rect.left+rect.width, 0.5f, rect.top));

        // We are dragging. Check for correct size
        //Vector3 size = transform.localScale + new Vector3(1, 0, 1);

        if (builderStage == BuildableRoom.STAGE_ITEMS)
        {
            //Debug.Log("items");
            // Placing items. Check We are placing it in the current room
            BuildableRoom room = (BuildableRoom)builder;
            BuildableItem placingItem = room.getCurrentBuildingItem();
            rect.width = (int)placingItem.width;
            rect.height = (int)placingItem.height;

            Vector3 scale = new Vector3(rect.width, 1, rect.height);
            transform.localScale = scale;

            //DRectangle innerRoom = room.collapse(1, 1, 1 ,1);
            // && data.navigation.canBeWalkedOn(rect)
            if (room.contains(rect) && canHaveItemsBuiltOn(rect))
            {
                // We are inside the room
                // Check we aren't clicking on an item already
                rend.material = selectionWorkMaterial;
                valid = true;
            }
            else
            {
                rend.material = selectionFailMaterial;
                valid = false;
            }
        }
        else
        {
            //DRectangle floorRect = rect.collapse(1, 1);
            if (rect.width < 5 || rect.height < 5 || data.dTileMap.collides(rect) || !canHaveRoomBuiltOn(rect))
            {
                rend.material = selectionFailMaterial;
                valid = false;
            }
            else {
                rend.material = selectionWorkMaterial;
                valid = true;
            }
        }
        rend.material.mainTextureScale = new Vector2(rect.width, rect.height);
    }

    private bool canHaveItemsBuiltOn(DRectangle rect)
    {
        for (int x = rect.left; x <= rect.right; x++)
        {
            for (int y = rect.top; y <= rect.bottom; y++)
            {
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
        doorPositions.Clear();
        windowPositions.Clear();
        foreach (Transform child in transform)
        {
            if (child.name != tileObject.name)
            {
                Destroy(child.gameObject);
            }
        }
        tileObject.transform.localScale = defaultTileObjectSize;
        tileObject.transform.localPosition = defulatTileObjectPosition;
        transform.localScale = Vector3.one;
    }

    public Buildable getBuildable()
    {
        return builder;
    }

    void OnEnable()
    {
        Debug.Log("Map -> " + FindObjectOfType<TileMapMouse>());
    }

    public bool isModified()
    {
        // Record what has changed 
        //Debug.Log(builder);
        buildStageChanged = this.builderStage != builder.getStage();
        buildPropertyChanged = this.builderProperty != builder.getProperty();
        bool roomChanged = false;
        if (builder is BuildableRoom){
            roomChanged = checkRoomChanged();
        }
        else if ( builder is BuildableTile)
        {
            roomChanged = checkTileChanged();
        }

        if ( buildPropertyChanged || buildStageChanged || roomChanged)
        {
            // Record changes and tell the graphics that we have changed
            this.builderStage = builder.getStage();
            this.builderProperty = builder.getProperty();
            return true;
        }

        // No Change
        return false;
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
        if (!containsAll(this.doorPositions, room.doors))
        {
            this.doorPositions.Clear();
            this.doorPositions.AddRange(room.doors);
            buildDoorsChanged = true;
            changed = true;
        }
        else
        {
            buildDoorsChanged = false;
        }


        // Windows
        if (!containsAll(this.windowPositions, room.windows))
        {
            Debug.Log("Data Window ADDED");
            this.windowPositions.Clear();
            this.windowPositions.AddRange(room.windows);
            buildWindowsChanged = true;
            changed = true;
        }
        else
        {
            buildWindowsChanged = false;
        }

        // Placed items
        if(!containsAll(placedItems, room.items))
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

        return true;
    }
}
