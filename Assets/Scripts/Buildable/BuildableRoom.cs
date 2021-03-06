﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class BuildableRoom : Buildable
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

    public GameObject roomObject;
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
    //protected GameObject selectionCube;

	public void OnEnable(){
		initialize ();
	}

    private void initialize()
    {

		selectionScript = GameObject.FindObjectOfType<SelectionTile>();
        if (selectionScript != null && selectionScript.rect != null)
        {
            selectionScript.rect.size = new Vector3(1, 0, 1);
        }

        Stage = STAGE_BLUEPRINT;
        Property = PROPERTY_BP_CREATE;

        data = GameObject.FindObjectOfType<GameData>();
        initialized = true;
    }

    public override void moveMouse(Vector3 movePosition)
    {
        if (!initialized) { initialize(); }

        //Debug.Log("Tile " + data.dTileMap.getTile((int)movePosition.x, (int)movePosition.z) + " " + movePosition);
        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_CREATE)
        {
            //Vector3 cubeSize = new Vector3(Mathf.Floor(selectionScript.rect.width / 2), 0, Mathf.Floor(selectionScript.rect.height / 2));
            selectionScript.rect.position = movePosition;
			selectionScript.rect.size = new Vector3(1,0,1);
        }
        else if (Stage == STAGE_ITEMS && Property == PROPERTY_ITEMS_PLACE)
        {
            //Vector3 cubeSize = new Vector3(Mathf.Floor(selectionScript.rect.width/2), 0, Mathf.Floor(selectionScript.rect.height/2));
            selectionScript.rect.position = movePosition;
        }
    }

    public BuildableItem getCurrentBuildingItem()
    {
        return editingItem;
    }

    public override void pressMouse(Vector3 pressPosition, MouseButton mouseButton)
    {
        if (!initialized) { initialize(); }

        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_RESIZE)
        {
            // Check if we clicked on the rectangle
            if (selectionScript.rect.contains(pressPosition))
            {
                // Clicked inside the rectangle. We can drag it now
                Property = PROPERTY_BP_MOVE;
                originalPosition = selectionScript.rect.position;
            }
            else {

                // If we want to recreate the floor. Let drag a new floor blueprint
                Property = PROPERTY_BP_CREATE;
            }
        }
    }

    public override void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition, MouseButton mouseButton)
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
            if (canPlaceDoor(releasePosition)){
                Vector2 position = new Vector2(Mathf.Ceil(releasePosition.x), Mathf.Ceil(releasePosition.z));
                
                DDoor door = DDoor.create(position, getDirection(position));
                doors.Add(door);
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
            if (canPlaceWindow(releasePosition))
            {
                Vector2 position = new Vector2(Mathf.Ceil(releasePosition.x), Mathf.Ceil(releasePosition.z));
                DWindow window = DWindow.create(position, getDirection(position));
                windows.Add(window);
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
                editingItem.left = (int)selectionScript.rect.left - selectionScript.rect.width  / 2;
                editingItem.top = (int)selectionScript.rect.top - selectionScript.rect.height / 2;
                addItem(editingItem);
                data.dTileMap.AddItem(editingItem);
                editingItem = (BuildableItem)ScriptableObject.CreateInstance(getPlaceableItems()[currentItemIndex].ToString());
                editingItem.Create(0, 0);
            }
            else if (pressedPosition.Equals(releasePosition))
            {
                // Can't build here. So check if it's an item
                foreach (BuildableItem item in items)
                {
                    if (item.contains(releasePosition))
                    {
                        //Debug.Log("Clicked on item " + selectionScript.rect.position);
                        Property = PROPERTY_ITEMS_EDIT;
                        editingItem = item;
                        editingItem.resetBuildTools();
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
                items[index].releaseMouse(pressedPosition, releasePosition, mouseButton);
            }
        }
        //Debug.Log("Property: " + Property);
        //Debug.Log("Stage: " + Stage);
    }

    private Navigation.Direction getDirection(Vector2 worldPosition)
    {
        Vector2 roomPosition = new Vector2(
            worldPosition.x - selectionScript.rect.left,
            worldPosition.y - selectionScript.rect.top
        );
        
        if (roomPosition.x == 0)
        {
            return Navigation.Direction.West;
        }
        else if (roomPosition.x == selectionScript.rect.width-1)
        {
            return Navigation.Direction.East;
        }
        else if (roomPosition.y == 0)
        {
            return Navigation.Direction.South;
        }
        else if (roomPosition.y == selectionScript.rect.height-1)
        {
            return Navigation.Direction.North;
        }

        Debug.Log("Invalid " + roomPosition + " - > " + selectionScript.rect.position + " " + selectionScript.transform.localScale);
        return Navigation.Direction.North;
    }

    private bool canPlaceDoor(Vector3 releasePosition)
    {
        float width = selectionScript.rect.width;
        float height = selectionScript.rect.height;
        Vector3 worldPosition = releasePosition - selectionScript.rect.position;

        // Make sure it's inside rect or not on the walls 
        if (!selectionScript.rect.contains(releasePosition) || selectionScript.rect.collapse(1, 1, 1, 1).contains(releasePosition))
        {
            return false;
        }

        
        if (doors.Contains(DDoor.create(worldPosition.x, worldPosition.z, getDirection(releasePosition))))
        {
            return false;
        }
        if ((worldPosition.x == 0 || worldPosition.x == width-1) && (worldPosition.z <= 0 || worldPosition.z >= height) )
        {
            return false;
        }
        if ((worldPosition.z == 0 || worldPosition.z == height-1) && (worldPosition.x <= 0 || worldPosition.x >= height))
        {
            return false;
        }

        return true;
    }

    private bool canPlaceWindow(Vector3 releasePosition)
    {
        float width = selectionScript.rect.width;
        float height = selectionScript.rect.height;
        Vector3 worldPosition = releasePosition - selectionScript.rect.position;

        // Make sure it's inside rect or not on the walls 
        if( !selectionScript.rect.contains(releasePosition) || selectionScript.rect.collapse(1,1,1,1).contains(releasePosition))
        {
            return false;
        }

        // Check we already have a door in this area
        if (doors.Contains(DDoor.create(new Vector2(worldPosition.x, worldPosition.z),getDirection(releasePosition))))
        {
            return false;
        }

        // Check we already have a window there
        if (windows.Contains(DWindow.create(new Vector2(worldPosition.x, worldPosition.z), getDirection(releasePosition))))
        {
            return false;
        }
        if ((worldPosition.x == 0 || worldPosition.x == width - 1) && (worldPosition.z <= 0 || worldPosition.z >= height))
        {
            return false;
        }
        if ((worldPosition.z == 0 || worldPosition.z == height - 1) && (worldPosition.x <= 0 || worldPosition.x >= width))
        {
            return false;
        }

        return true;
    }

    protected virtual void addItem(BuildableItem clone)
    {
        items.Add(clone);
    }

    public override void dragMouse(Vector3 pressedPosition, Vector3 dragPosition)
    {
        if (!initialized) { initialize(); }

        Vector3 change = dragPosition - pressedPosition;

        int right = (int)Mathf.Max(dragPosition.x, pressedPosition.x) + 1;
        int bottom = (int)Mathf.Min(dragPosition.z, pressedPosition.z);
        int left = (int)Mathf.Min(dragPosition.x, pressedPosition.x);
        int top = (int)Mathf.Max(dragPosition.z, pressedPosition.z);
        int width = (int)(right - left);
        int height = (int)(top - bottom) + 1;
        /*this.top = bottom;
        this.left = left;
        this.width = width;
        this.height = height;*/

        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_CREATE)
        {
            // Move cube to the right space and scale it
            selectionScript.rect.position = new Vector3(left, 0, bottom);
            selectionScript.rect.size = new Vector3(width, 1, height);
        }
        else if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_RESIZE)
        {

        }
        else if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_MOVE)
        {
            // We are moving this rectangle
            selectionScript.rect.position = originalPosition + change;
            //this.top = (int)(selectionScript.rect.top-height);
            //this.left = (int)selectionScript.rect.left;
        }
    }

    public override void applyStage()
    {
        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_RESIZE)
        {
            Stage = STAGE_WINDOWSDOORS;
            Property = PROPERTY_WD_DOOR;
        }
        else if (Stage == STAGE_WINDOWSDOORS)
        {
            if (canBeBuilt())
            {
                this.width = (int)selectionScript.rect.width;
                this.height = (int)selectionScript.rect.height;
                this.left = (int)selectionScript.rect.left;
                this.top = (int)selectionScript.rect.top;

                data.dTileMap.ApplyToTileMap(this);

                Property = PROPERTY_ITEMS_PLACE;
                Stage = STAGE_ITEMS;
                currentItemIndex = 0;

                editingItem = ((BuildableItem)ScriptableObject.CreateInstance(getPlaceableItems()[currentItemIndex].ToString()));
                editingItem.Create(0, 0);
                //Debug.Log("Eiditing " + editingItem);

                //selectionCube.transform.GetChild(0).localScale = new Vector3(1, 0.2f, 1);
                //selectionCube.transform.GetChild(0).localPosition = new Vector3(0.5f, 0.125f, 0.5f);
                //selectionCube.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        Debug.Log("Property: " + Property);
        Debug.Log("Stage: " + Stage);
    }

    public override void cancelStage()
    {
        if (Stage == STAGE_BLUEPRINT && Property == PROPERTY_BP_RESIZE)
        {

        }
        else if (Stage == STAGE_WINDOWSDOORS)
        {
            Stage = STAGE_BLUEPRINT;
            Property = PROPERTY_BP_CREATE;
            selectionScript.rect.width = 1;
            selectionScript.rect.height = 1;
            doors.Clear();
            windows.Clear();
            walls.Clear();
        }
        else if (Stage == STAGE_ITEMS && Property == PROPERTY_ITEMS_PLACE)
        {
            // If we are editing an item, delete it
            if (items.Count > 0)
            {
                while (items.Count > 0)
                {
                    BuildableItem i = items[0];
                    data.dTileMap.RemoveItem(i);
                    items.Remove(i);
                }
            }
            else
            {
                selectionScript.rect.left  = left;
                selectionScript.rect.top   = top;
                selectionScript.rect.width = width;
                selectionScript.rect.height = height;
                data.dTileMap.RemoveFromTileMap(this);
                Stage = STAGE_WINDOWSDOORS;
                Property = PROPERTY_WD_DOOR;
                Debug.Log("Rect " + selectionScript.rect);
            }

        }
        else if(Stage == STAGE_ITEMS && Property == PROPERTY_ITEMS_EDIT)
        {
            // Not editing an item, Delete all items and go back to window doors
            int index = items.IndexOf(editingItem);
            data.dTileMap.RemoveItem(items[index]);
            items.Remove(items[index]);
            Property = PROPERTY_ITEMS_PLACE;
        }
        else
        {
            Debug.Log("Unknown Stage " + Stage + " and property " + Property);
        }
    }

    public override int getStage()
    {
        return Stage;
    }

    public override string getProperty()
    {
        return Property;
    }

    public override bool hasNextStage()
    {
        return Stage != STAGE_ITEMS || !finishedPlacingItems();
    }

    public DWall getWall(int x, int y)
    {
        //Debug.Log("Get Wall " + x + "," + y);
        foreach (DWall wall in walls)
        {
            //Debug.Log("Wall " + wall.position);
            if (wall.position.x == x && wall.position.y == y)
            {
                return wall;
            }
        }
        //Debug.LogError("Couldn't Find Wall " + x + "," + y);
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

    public override void switchValue()
    {
        currentItemIndex++;
        if (currentItemIndex >= getPlaceableItems().Count)
        {
            currentItemIndex = 0;
        }
        editingItem = (BuildableItem)ScriptableObject.CreateInstance(getPlaceableItems()[currentItemIndex].ToString());
        editingItem.Create(0, 0);
    }

    private bool finishedPlacingItems()
    {
        // Create dictionary of what we have
        Dictionary<Type, int> placedItems = new Dictionary<Type, int>();
        foreach(BuildableItem item in items)
        {
            Type key = item.GetType();
            if (placedItems.ContainsKey(key))
            {
                placedItems[key] += 1;
            }
            else
            {
                placedItems.Add(key, 1);
            }
        }

        // Compare it
        foreach(Type i in getRequiredItems().Keys)
        {
            bool contains = false;
            foreach (Type j in placedItems.Keys)
            {
                if( j == i || j.IsSubclassOf(i))
                {

                    if (placedItems[j] < getRequiredItems()[i])
                    {
                        Debug.Log("Count of " + i + " " + placedItems[j] + " < " + getRequiredItems()[i]);
                        return false;
                    }
                    contains = true;
                }
            }


            if ( !contains )
            {
                Debug.Log("User has not placed " + i);
                return false;
            }
        }

        // Acceptable
        return true;

    }

    public override bool Equals(object obj)
    {
        if (!base.Equals(obj))
        {
            return false;
        }
        if ( !(obj is Buildable) )
        {
            return false;
        }

        return true;
    }

    public virtual void RemoveItem(BuildableItem item)
    {
        items.Remove(item);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    
    public abstract List<Type> getPlaceableItems();
    public abstract Dictionary<Type, int> getRequiredItems();
}
