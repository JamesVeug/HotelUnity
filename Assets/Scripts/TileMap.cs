using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour
{

    protected class TileMapImage{
        public Color[] pixels;
        public int width;
        public int height;
    }

    /*public Material temp;
    public int previousTemp;
    public float time;*/

    public int width = 50;
    public int height = 50;
    public float tileSize = 1.0f;
    public float resolution = 8f;
    public Material itemMaterial;
    public Material[] materials;
    public GameObject wallPrefab;
    public GameObject wallCornerPrefab;
    public GameObject doorPrefab;
    public GameObject windowPrefab;
    public GameObject[] itemObjects;
    public Type[] itemScripts = {
        typeof(BuildableMinibar),
        typeof(BuildableReception),
        typeof(BuildableChair),
        typeof(BuildablePainting),
        typeof(ISingleBed), // Clean
        typeof(ISingleBed), // InUse
        typeof(ISingleBed), // Dirty
        typeof(IDoubleBed), // Clean
        typeof(IDoubleBed), // Dirty
    };


    public GameObject blueprintWallPrefab;
    public GameObject blueprintDoorPrefab;
    public GameObject blueprintWindowPrefab;

    private GameData gameData;
    private Material[] currentMaterials;
    private TileMapImage[] imageMaps;
    private int texWidth;
    private int texHeight;
    // Create map copy 
    // When we add buildings only change the pixels of the changed tiles

    // Built map
    private DTileMap map;
    private SelectionTile selectionScript;

    // Use this for initialization
    void Start()
    {
        BuildMesh();
        selectionScript = FindObjectOfType<SelectionTile>();
    }

    void Update()
    {

       /* MeshRenderer rendtemp = GetComponent<MeshRenderer>();
        if(rendtemp.materials.Length > 1 && time < Time.time)   
        {
            rendtemp.materials[previousTemp].color = temp.color;
            previousTemp++;
            if( previousTemp >= width*height)
            {
                previousTemp = 0;
            }
            rendtemp.materials[previousTemp].color = Color.red;
            time = Time.time + 1;
        }*/

        if ( map.changes == null)
        {
            Debug.LogError("Map.Changes is null");
        }
        else if (map.changes.Count > 0)
        {

            MeshRenderer rend = GetComponent<MeshRenderer>();
            // Every change in the game
            foreach (Vector2 c in map.changes)
            {
                int x = (int)c.x;
                int y = (int)c.y;
                int index = x * height + y;
                
                // Change Material
                DTile tile = map.getTile(x,y);
                if (!tile.item)
                {
                    int type = tile.tileType;
                    currentMaterials[index] = materials[type];
                }
                else
                {
                    currentMaterials[index] = itemMaterial;
                }
                rend.materials = currentMaterials;


                //Debug.Log(x + "," + y + "-> " + index + " " + materials[tile]);

                // Wall types
                if ( map.hasWall(x, y) )
                {
                    DWall wall = map.getWall(x, y);
                    if( wall == null){Debug.LogError("No Wall at position " + x + "," + y);}
                    buildWall(wall);
                }
                else if (map.hasDoor(x, y))
                {
                    DDoor door = map.getDoor(x, y);
                    if (door == null) { Debug.LogError("No door at position " + x + "," + y); }
                    buildDoor(door);
                }
                else if (map.hasWindow(x, y))
                {
                    DWindow window = map.getWindow(x, y);
                    if (window == null) { Debug.LogError("No window at position " + x + "," + y); }
                    buildWindow(window);
                }

                // Items are different to walls
                if (map.hasItem(x, y))
                {
                    BuildableItem item = map.getItem(x, y);
                    if (item == null) { Debug.LogError("No item at position " + x + "," + y); }
                    //Debug.Log("Placed Item at " + x + "," + y);
                    buildItem(item);
                }
            }
            map.changes.Clear();
        }

        // STYLE Selection box
        if (selectionScript.isModified())
        {
            if (selectionScript.getBuildable() is BuildableRoom) {
                BuildableRoom build = (BuildableRoom)selectionScript.getBuildable();
                int stage = build.getStage();
                string property = build.getProperty();

                // Selection cube has changed. We need to style it
                if (stage == BuildableRoom.STAGE_BLUEPRINT )
                {
                    //Transform selectionCube = selectionScript.transform.GetChild(0);
                    if (property == BuildableRoom.PROPERTY_BP_CREATE)
                    {
                        //selectionCube.localScale = new Vector3(1, 0.2f, 1);
                        //selectionCube.position = selectionCube.localScale/2;
                    }
                    else if( property == BuildableRoom.PROPERTY_BP_RESIZE)
                    {
                        //selectionCube.localScale = new Vector3(1, 2.5f, 1);
                        //selectionCube.position += new Vector3(0, selectionCube.localScale.y / 2, 0);
                    }
                    else if (property == BuildableRoom.PROPERTY_BP_MOVE)
                    {
                        //Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                    }
                }
                else if(stage == BuildableRoom.STAGE_WINDOWSDOORS)
                {
                    Transform selectionCube = selectionScript.transform;



                    Transform bPParent = selectionCube.FindChild("Blueprints");
                    GameObject o = null;
                    if (bPParent == null)
                    {
                        o = new GameObject();
                        o.transform.parent = selectionCube;
                        o.name = "Blueprints";
                    }
                    else
                    {
                        o = bPParent.gameObject;
                    }


                    // If we just changed to this Stage. Build the meshes
                    if (selectionScript.stageModified())
                    {
                        // We are now changing the windows and doors
                        // Show each wall
                        
                        // Create new walls
                        Vector3 position = selectionScript.rect.position;
                        for (int x = 0; x < selectionScript.rect.width; x++)
                        {
                            for (int y = 0; y <= 1; y++)
                            {
                                Vector3 newPos = new Vector3(x, 0, y * (selectionScript.rect.height - 1));
                                GameObject bpWall = (GameObject)Instantiate(blueprintWallPrefab);
                                bpWall.transform.position = position + newPos;
                                bpWall.transform.parent = o.transform;
                                bpWall.name = "BPWall" + newPos.x + "," + newPos.z;
                                rotateByBounds(bpWall, selectionScript.rect);
                            }
                        }
                        for (int y = 0; y < selectionScript.rect.height; y++)
                        {
                            for (int x = 0; x <= 1; x++)
                            {
                                Vector3 newPos = new Vector3(x * (selectionScript.rect.width - 1), 0, y);
                                GameObject bpWall = (GameObject)Instantiate(blueprintWallPrefab);
                                bpWall.transform.position = position + newPos;
                                bpWall.transform.parent = o.transform;
                                bpWall.name = "BPWall" + newPos.x + "," + newPos.z;
                                rotateByBounds(bpWall, selectionScript.rect);
                            }
                        }

                        // Corners // HACK HACK HACK HACK
                        // Top Left
                        for (int y = 0; y <= 1; y++)
                        {
                            for (int x = 0; x <= 1; x++)
                            {
                                Vector3 newPos = new Vector3(x * (selectionScript.rect.width - 1), 0, y * (selectionScript.rect.height - 1));
                                GameObject bpWall = (GameObject)Instantiate(blueprintWallPrefab);
                                bpWall.transform.position = position + newPos;
                                bpWall.transform.parent = o.transform;
                                bpWall.name = "BPWallCorner" + newPos.x + "," + newPos.z;
                                if ( y == 1)
                                {
                                    bpWall.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                }
                            }
                        }
                    }

                    // Check if the door positions
                    if (selectionScript.doorsModified())
                    {
                        //Debug.Log("DOORS CHANGED");
                        // Get the position
                        foreach (DDoor door in build.doors)
                        {
                            Vector3 newPos = new Vector3(door.position.x, 0, door.position.y);
                            Vector3 roomPos = newPos - selectionScript.rect.position;

                            // Remove anything that was originally there
                            foreach (Transform child in selectionCube)
                            {
                                if (child.name.EndsWith(roomPos.x + "," + roomPos.z))
                                {
                                    Destroy(child.gameObject);
                                    break;
                                }
                            }


                            // Add the door
                            GameObject bpDoor = (GameObject)Instantiate(blueprintDoorPrefab);
                            bpDoor.transform.position = newPos;
                            bpDoor.transform.parent = o.transform;
                            bpDoor.name = "BPDoor" + newPos.x + "," + newPos.z;
                            rotateByBounds(bpDoor, selectionScript.rect);
                        }
                    }

                    // Get the position
                    if (selectionScript.windowsModified())
                    {
                        foreach (DWindow window in build.windows)
                        {
                            Vector3 newPos = new Vector3(window.position.x, 0, window.position.y);
                            Vector3 roomPos = newPos - selectionScript.rect.position;

                            // Remove anything that was originally there
                            foreach (Transform child in selectionCube)
                            {
                                if (child.name.EndsWith(roomPos.x + "," + roomPos.z))
                                {
                                    Destroy(child.gameObject);
                                    break;
                                }
                            }


                            // Add the Window
                            GameObject bpWindow = (GameObject)Instantiate(blueprintWindowPrefab);
                            bpWindow.transform.position = newPos;
                            bpWindow.transform.parent = o.transform;
                            bpWindow.name = "BPWindow" + newPos.x + "," + newPos.z;
                            rotateByBounds(bpWindow, selectionScript.rect);
                        }
                    }
                }
                else if(stage == BuildableRoom.STAGE_ITEMS)
                {
                    if (selectionScript.stageModified())
                    {
                        Transform selectionCube = selectionScript.transform;
                        Destroy(selectionCube.FindChild("Blueprints").gameObject);
                    }
                    else if( selectionScript.placedItemsModified() )
                    {

                    }
                }
            }
        }
    }

    public void buildTexture(Mesh mesh)
    {

        MeshRenderer rend = GetComponent<MeshRenderer>();
        int numTiles = width * height;
        int numTriangles = numTiles * 2;
        
        Material starting = materials[0];

        currentMaterials = new Material[numTriangles];
        for (int i = 0; i < numTriangles; i++)
        {
            currentMaterials[i] = Instantiate(starting);
        }
        rend.materials = currentMaterials;

    }

    public void BuildMesh()
    {
        gameData = FindObjectOfType<GameData>();
        gameData.graphicsMap = this;
        map = gameData.createTileMap(width, height);

        int numTiles = width * height;
        int numTriangles = numTiles * 2;

        // Vertex Calculation
        int numVerticies = numTriangles*6;

        // Generate mesh data
        Vector3[] verticies = new Vector3[numVerticies];
        List<int[]> triangles = new List<int[]>(numTriangles + 1);
        Vector3[] normals = new Vector3[verticies.Length];
        Vector2[] uvs = new Vector2[verticies.Length];

        float x = 0;
        float y = 0;
        for (int t = 0; t < numTriangles; t++)
        {

            int vIndex = t * 6;

            // Triangle 1
            verticies[vIndex + 0] = new Vector3(x, 0, y);
            verticies[vIndex + 1] = new Vector3(x, 0, y + tileSize);
            verticies[vIndex + 2] = new Vector3(x + tileSize, 0, y);
            normals[vIndex + 0 ] = Vector3.up;
            normals[vIndex + 1 ] = Vector3.up;
            normals[vIndex + 2 ] = Vector3.up;
            uvs[vIndex + 0] = new Vector2(0,0);
            uvs[vIndex + 1] = new Vector2(0, 1);
            uvs[vIndex + 2] = new Vector2(1, 0);

            // Triangle 2
            verticies[vIndex + 3] = new Vector3(x + tileSize, 0, y);
            verticies[vIndex + 4] = new Vector3(x, 0, y + tileSize);
            verticies[vIndex + 5] = new Vector3(x + tileSize, 0, y + tileSize);
            normals[vIndex + 3] = Vector3.up;
            normals[vIndex + 4] = Vector3.up;
            normals[vIndex + 5] = Vector3.up;
            uvs[vIndex + 3] = new Vector2(1, 0);
            uvs[vIndex + 4] = new Vector2(0, 1);
            uvs[vIndex + 5] = new Vector2(1, 1);

            int[] triangle = new int[6];
            triangle[0] = vIndex + 0;
            triangle[1] = vIndex + 1;
            triangle[2] = vIndex + 2;
            triangle[3] = vIndex + 3;
            triangle[4] = vIndex + 4;
            triangle[5] = vIndex + 5;
            

            triangles.Add(triangle);

            y += tileSize;
            if( y >= tileSize*width)
            {
                x += (int)tileSize;
                y = 0;
            }
        }


        // Create mesh and populate it
        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.subMeshCount = numTiles;
        for (int i = 0; i < numTiles; i++)
        {
            mesh.SetTriangles(triangles[i], i);
        }
        mesh.normals = normals;
        mesh.uv = uvs;

        // Assign out mesh to the object
        MeshFilter mesh_filter = GetComponent<MeshFilter>();
        MeshCollider mesh_collider = GetComponent<MeshCollider>();

        mesh_filter.mesh = mesh;
        mesh_collider.sharedMesh = mesh;

        // Build textures
        buildTexture(mesh);
    }

    private void buildWall(DWall wall)
    {
        BuildableRoom room = map.getRoom((int)wall.position.x, (int)wall.position.y);
        Vector3 position = new Vector3(wall.position.x, 0, wall.position.y);

        Vector3 roomPosition = position - room.position;
        GameObject wallObject;
        if((roomPosition.x == 0 || roomPosition.x == (room.width-1) ) && (roomPosition.z == 0 || roomPosition.z == (room.height-1)))
        {
            wallObject = (GameObject)Instantiate(wallCornerPrefab);
            if( (roomPosition.x == 0 && roomPosition.z == 0) || (roomPosition.x == (room.width - 1) && roomPosition.z == (room.height-1)) )
            {
                wallObject.transform.GetChild(0).localRotation *= Quaternion.Euler(0, -90, 0);
            }
        }
        else
        {
            wallObject = (GameObject)Instantiate(wallPrefab);
        }

        wallObject.transform.position = new Vector3(wall.position.x, 0, wall.position.y);
        wall.gameObject = wallObject;
        rotateByObject(wall);
    }

    private void rotateByObject(DDataObject o)
    {
        if( o.facingDirection == Navigation.Direction.North)
        {
            o.gameObject.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 180, 0);
        }
        else if (o.facingDirection == Navigation.Direction.East)
        {
            o.gameObject.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 270, 0);
        }
        else if (o.facingDirection == Navigation.Direction.West)
        {
            o.gameObject.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 90, 0);
        }
        else
        {
            // South
        }
    }

    private void buildWindow(DWindow window)
    {
        GameObject windowObject = (GameObject)Instantiate(windowPrefab);
        windowObject.transform.position = new Vector3(window.position.x, 0, window.position.y);

        window.gameObject = windowObject;
        rotateByObject(window);
    }

    private void buildDoor(DDoor door)
    {
        GameObject doorObject = (GameObject)Instantiate(doorPrefab);
        doorObject.transform.position = new Vector3(door.position.x, 0, door.position.y);

        door.gameObject = doorObject;
        rotateByObject(door);
    }

    private void buildItem(BuildableItem item)
    {
        //Debug.Log("Item " + item.GetType().Name);
        string name = item.GetType().Name + item.position.x + "," + item.position.z;
        GameObject itemObject = GameObject.Find(name);
        int index = 0;
        if (itemObject == null)
        {
            for(int i = 0; i < itemScripts.Length;i++)
            {

                if( itemScripts[i] == item.GetType())
                {
                    //Debug.Log("Index " + i);
                    itemObject = (GameObject)Instantiate(itemObjects[i]);
                    itemObject.name = name;
                    index = i;
                    break;
                }
            }
            if( itemObject == null)
            {
                // No possible combinations 
                Debug.LogError("Could not match type of " + item.GetType());
            }

            // Bed types
            if (item is BuildableBed)
            {
                BuildableBed bed = (BuildableBed)item;
                GameObject dirtyObject = (GameObject)Instantiate(itemObjects[index + 1]);
                dirtyObject.name = name + "(Dirty)";
                dirtyObject.transform.position = new Vector3((int)item.position.x, 0, (int)item.position.z);
                dirtyObject.transform.GetChild(0).transform.rotation = item.rotation;
                dirtyObject.SetActive(false);
                bed.dirtyGameObject = dirtyObject;

                GameObject inUseObject = (GameObject)Instantiate(itemObjects[index + 2]);
                inUseObject.name = name + "(InUse)";
                inUseObject.transform.position = new Vector3((int)item.position.x, 0, (int)item.position.z);
                inUseObject.transform.GetChild(0).transform.rotation = item.rotation;
                inUseObject.SetActive(false);
                bed.inUseGameObject = inUseObject;
                Debug.Log("BuildableBed " + (index + 1));
            }
        }
        itemObject.transform.position = new Vector3((int)item.position.x, 0, (int)item.position.z);
        itemObject.transform.GetChild(0).transform.rotation = item.rotation;
        item.gameObject = itemObject;

        if (itemObject == null)
        {
            Debug.LogError("item.gameObject " + item.gameObject);
        }




    }

    private void rotateByBounds(GameObject o, DRectangle room)
    {
        Vector3 position = o.transform.position - room.position;
        if (position.x == 0)
        {
            o.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 90, 0);
        }
        else if (position.x == room.width - 1)
        {
            o.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 270, 0);
        }
        else if (position.z == 0)
        {
            o.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 0, 0);
        }
        else if (position.z == room.height - 1)
        {
            o.transform.GetChild(0).localRotation *= Quaternion.Euler(0, 180, 0);
        }
    }
}
