using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class BuildableRoom : DRectangle, Buildable
{
    public const int STAGE_BLUEPRINT = 0;
    public const int STAGE_WINDOWSDOORS = 1;
    public const int STAGE_ITEMS = 2;

    public const string PROPERTY_BP_CREATE = "CREATE";
    public const string PROPERTY_BP_RESIZE = "RESIZE";
    public const string PROPERTY_BP_MOVE = "MOVE";


    public const string PROPERTY_WD_DOOR = "DOOR";
    public const string PROPERTY_WD_WINDOW = "Window";

    public const string PROPERTY_ITEMS_PLACE = "PlaceItem";
    public const string PROPERTY_ITEMS_EDIT = "EditItem";

    public int maxDoors = 2;
    private int currentItemIndex;
    private BuildableItem editingItem;
    private bool initialized = false;

    //private Vector3 lastPoint = Vector3.zero;
    private Vector3 originalPosition = Vector3.zero;

    public List<DDoor> doors = new List<DDoor>();
    public List<DWindow> windows = new List<DWindow>();
    public List<BuildableItem> items = new List<BuildableItem>();
    public List<DWall> walls = new List<DWall>();

    // Blueprint
    // Windows/Doors
    // Items
    protected int Stage = 0; // Blueprint - Windows/doors - Items

    // Blueprint
    // - Create Floor
    // - Resize
    // Windows/Doors
    protected string Property = PROPERTY_BP_CREATE; // Options of current Stage


    protected GameData data;
    protected SelectionTile selectionScript;
    protected GameObject selectionCube;
    protected BuildableRoom() : this(0, 0, 0, 0)
    {

    }

    protected BuildableRoom(int x, int y, int width, int height) : base(x, y, width, height)
    {
        
    }

    private void initialize()
    {

        selectionScript = GameObject.FindObjectOfType<SelectionTile>();
        selectionCube = selectionScript.gameObject;

        Stage = STAGE_BLUEPRINT;
        Property = PROPERTY_BP_CREATE;

        data = GameObject.FindObjectOfType<GameData>();
        initialized = true;
    }

    public void moveMouse(Vector3 movePosition)
    {
        if (!initialized) { initialize(); }

        //Debug.Log("Tile " + data.dTileMap.getTile((int)movePosition.x, (int)movePosition.z) + " " + movePosition);
        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_CREATE)
        {
            Vector3 cubeSize = new Vector3(Mathf.Floor(selectionCube.transform.localScale.x / 2), 0, Mathf.Floor(selectionCube.transform.localScale.z / 2));
            selectionCube.transform.position = movePosition - cubeSize;
        }
        else if (Stage == STAGE_ITEMS)
        {
            Vector3 cubeSize = new Vector3(Mathf.Floor(selectionCube.transform.localScale.x/2), 0, Mathf.Floor(selectionCube.transform.localScale.z/2));
            selectionCube.transform.position = movePosition - cubeSize;
        }
    }

    public BuildableItem getCurrentBuildingItem()
    {
        return getPlaceableItems().Count == 0 ? null : getPlaceableItems()[currentItemIndex];
    }

    public void pressMouse(Vector3 pressPosition)
    {
        if (!initialized) { initialize(); }

        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_RESIZE)
        {
            this.left = (int)selectionCube.transform.position.x;
            this.top = (int)selectionCube.transform.position.z;
            this.width = (int)selectionCube.transform.localScale.x;
            this.height = (int)selectionCube.transform.localScale.z;
            //Debug.DrawLine(new Vector3(left, 1, top), new Vector3(left, 1, bottom), Color.blue,10);
            //Debug.DrawLine(new Vector3(left, 1, top), new Vector3(right, 1, top), Color.red, 10);


            // Check if we clicked on the rectangle
            if (this.contains(pressPosition))
            {
                // Clicked inside the rectangle. We can drag it now
                Property = PROPERTY_BP_MOVE;
                originalPosition = selectionCube.transform.position;
            }
            else {

                // If we want to recreate the floor. Let drag a new floor blueprint
                Property = PROPERTY_BP_CREATE;
            }
        }
    }

    public void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition)
    {
        if (!initialized) { initialize(); }

        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_CREATE)
        {
            // Finished creating the Floor blueprint. Let us resize it now
            Property = PROPERTY_BP_RESIZE;
        }
        else if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_MOVE)
        {
            // Finished moving the rectangle
            Property = PROPERTY_BP_RESIZE;
        }
        else if (Stage == STAGE_WINDOWSDOORS && Property == PROPERTY_WD_DOOR)
        {
            // Add door
            Vector3 worldPosition = releasePosition - selectionCube.transform.position;
            if ((worldPosition.x == 0 || worldPosition.x == width - 1 || worldPosition.z == 0 || worldPosition.z == height - 1) && !doors.Contains(new DDoor(new Vector2(worldPosition.x, worldPosition.z))))
            {
                Vector2 position = new Vector2(left + worldPosition.x, top + worldPosition.z);

                doors.Add(new DDoor(position));
                data.dTileMap.changes.Add(new Vector2(position.x, position.y));
                if (doors.Count >= maxDoors)
                {
                    Property = PROPERTY_WD_WINDOW;
                }
            }
        }
        else if (Stage == STAGE_WINDOWSDOORS && Property == PROPERTY_WD_WINDOW)
        {
            // Add Window
            Vector3 worldPosition = releasePosition - selectionCube.transform.position;
            if ((worldPosition.x == 0 || worldPosition.x == width - 1 || worldPosition.z == 0 || worldPosition.z == height - 1) && !windows.Contains(new DWindow(new Vector2(worldPosition.x, worldPosition.z))))
            {
                windows.Add(new DWindow(new Vector2(left + worldPosition.x, top + worldPosition.z)));
            }
        }
        else if (Stage == STAGE_ITEMS && Property == PROPERTY_ITEMS_PLACE)
        {

            // Left click place. 
            // Right click change item
            //if( Input.mouse)

            // Check we clicked on an item. Then rotate it
            if (selectionScript.isValid())
            {
                // Add Item
                BuildableItem clone = (BuildableItem)getPlaceableItems()[currentItemIndex].Clone();
                clone.left = (int)selectionCube.transform.position.x;
                clone.top = (int)selectionCube.transform.position.z;
                items.Add(clone);
                data.dTileMap.AddItem(clone);
            }
            else if (pressedPosition.Equals(releasePosition))
            {
                // Can't build here. So check if it's an item
                foreach (BuildableItem item in items)
                {
                    if (item.contains(releasePosition))
                    {
                        //Debug.Log("Clicked on item");
                        Property = PROPERTY_ITEMS_EDIT;
                        editingItem = item;
                        break;
                    }
                }
            }
        }
        else if (Stage == STAGE_ITEMS && Property == PROPERTY_ITEMS_EDIT)
        {
            if (!editingItem.contains(releasePosition))
            {
                Property = PROPERTY_ITEMS_PLACE;
            }
            else
            {
                int index = items.IndexOf(editingItem);
                items[index].rotation = editingItem.rotation * Quaternion.Euler(0, 90, 0);
                // Add more changes
                data.dTileMap.changes.Add(new Vector2(items[index].getOrigin().x, items[index].getOrigin().y));
                Debug.Log("Rotating BuildingRoom");
            }
        }
        //Debug.Log("Property: " + Property);
        //Debug.Log("Stage: " + Stage);
    }

    public void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        if (!initialized) { initialize(); }

        Vector3 change = dragPosition - pressedPosition;

        int right = (int)Mathf.Max(dragPosition.x, pressedPosition.x) + 1;
        int bottom = (int)Mathf.Min(dragPosition.z, pressedPosition.z);
        int left = (int)Mathf.Min(dragPosition.x, pressedPosition.x);
        int top = (int)Mathf.Max(dragPosition.z, pressedPosition.z);
        int width = (int)(right - left);
        int height = (int)(top - bottom) + 1;
        this.top = bottom;
        this.left = left;
        this.width = width;
        this.height = height;


        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_CREATE)
        {
            // Move cube to the right space and scale it
            selectionCube.transform.position = new Vector3(left, 0, bottom);
            selectionCube.transform.localScale = new Vector3(width, 1, height);
        }
        else if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_RESIZE)
        {

        }
        else if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_MOVE)
        {
            // We are moving this rectangle
            selectionCube.transform.position = originalPosition + change;
        }
    }

    public void applyStage()
    {
        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_RESIZE)
        {
            //selectionCube.transform.GetChild(0).localScale += new Vector3(0,2.4f,0);
            //selectionCube.transform.position += new Vector3(0, 1.2f, 0);
            Stage = STAGE_WINDOWSDOORS;
            Property = PROPERTY_WD_DOOR;
        }
        else if (Stage == STAGE_WINDOWSDOORS)
        {
            if (canBeBuilt())
            {
                this.width = (int)selectionCube.transform.localScale.x;
                this.height = (int)selectionCube.transform.localScale.z;
                this.left = (int)selectionCube.transform.position.x;
                this.top = (int)selectionCube.transform.position.z;

                data.dTileMap.MakeRoom(this);

                Property = PROPERTY_ITEMS_PLACE;
                Stage = STAGE_ITEMS;
                currentItemIndex = 0;


                selectionCube.transform.GetChild(0).localScale = new Vector3(1, 0.2f, 1);
                selectionCube.transform.GetChild(0).localPosition = new Vector3(0.5f, 0.125f, 0.5f);
                selectionCube.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        Debug.Log("Property: " + Property);
        Debug.Log("Stage: " + Stage);
    }

    public int getStage()
    {
        return Stage;
    }

    public string getProperty()
    {
        return Property;
    }

    public bool hasNextStage()
    {
        return Stage != STAGE_ITEMS;
    }

    public DWall getWall(int x, int y)
    {
        foreach (DWall wall in walls)
        {
            if (wall.position.x == x && wall.position.y == y)
            {
                return wall;
            }
        }
        return null;
    }

    public DDoor getDoor(int x, int y)
    {
        foreach (DDoor door in doors)
        {
            if (door.position.x == x && door.position.y == y)
            {
                return door;
            }
        }
        return null;
    }

    public BuildableItem getItem(int x, int y)
    {
        foreach (BuildableItem item in items)
        {
            if (item.contains(new Vector3(x, 0, y)))
            {
                return item;
            }
        }
        return null;
    }

    public DWindow getWindow(int x, int y)
    {
        foreach (DWindow window in windows)
        {
            if (window.position.x == x && window.position.y == y)
            {
                return window;
            }
        }
        return null;
    }

    public void switchValue()
    {
        Debug.Log("SWITCHING");
        currentItemIndex++;
        if (currentItemIndex >= getPlaceableItems().Count)
        {
            currentItemIndex = 0;
        }
    }

    public abstract bool canBeBuilt();
    public abstract List<BuildableItem> getPlaceableItems();
}
